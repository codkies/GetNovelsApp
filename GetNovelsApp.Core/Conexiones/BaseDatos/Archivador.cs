using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Utilidades;
using Org.BouncyCastle.Asn1.Cmp;

namespace GetNovelsApp.Core.Conexiones.DB
{
    /// <summary>
    /// Query maker
    /// </summary>
    public partial class Archivador : IReportero
    {
        #region fields
        private const string TablaNovelas = "Novelas";
        private const string TablaCapitulos = "Capitulos";
        private const string TablaClasificacion = "Clasificacion";
        private const string TablaAutor = "Autores";

        /// <summary>
        /// Define si se están guardando capitulos
        /// </summary>
        private bool EjecutandoGuardado = false;

        public string Nombre => "DB";
        #endregion


        #region Core


        Dictionary<Capitulo, int> CapitulosAGuardar = new Dictionary<Capitulo, int>();


        /// <summary>
        /// Toma una informacion de novela online y la guarda.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<INovela> MeteNovelaDBAsync(InformacionNovelaOnline info)
        {
            bool YaExiste = NovelaExisteEnDB(info.LinkPrincipal);
            if (!YaExiste)
            {
                using IDbConnection cnn = DataBaseAccess.GetConnection();
                //Obteniendo toda la info de la web y metiendola en la DB:
                InformacionNovelaDB novDBInfo = InsertaNovelaEnDB(info, cnn);

                //Capitulos:
                List<Capitulo> CapitulosNovela = GetNovelsFactory.FabricaCapitulos(info.LinksDeCapitulos);
                await GuardaCapitulosAsync(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                INovela nov = GetNovelsFactory.FabricaNovela(CapitulosNovela, novDBInfo);
                cnn.Dispose();
                return nov;
            }
            else
            {
                return SacaNovelaDB(info.LinkPrincipal);
            }
        }


        public async Task GuardaCapitulosAsync(Capitulo capitulo, int novelaID)
        {
            CapitulosAGuardar.Add(capitulo, novelaID);
            if (EjecutandoGuardado) return;
            await Task.Run(() => CapitulosAGuardar_Ejecuta());
        }


        public async Task GuardaCapitulosAsync(List<Capitulo> capitulos, int novelaID)
        {
            capitulos.ForEach(x => CapitulosAGuardar.Add(x, novelaID));
            if (EjecutandoGuardado) return;
            await Task.Run(() => CapitulosAGuardar_Ejecuta());
        }


        /// <summary>
        /// Regresa true or false dependiendo si el link ingresado, corresponde a alguna novela en la DB.
        /// </summary>
        /// <param name="Link"></param>
        /// <returns></returns>
        public bool NovelaExisteEnDB(Uri Link)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            string qry = $"select NovelaID from {i.TNovelas} as n " +
                            $"join {i.TLinks} as l " +
                               $"on n.NovelaID = l.NovelaID and l.Link = '{Link}'";
            var resultados = cnn.Query<int>(qry);
            cnn.Dispose();

            return resultados.Any();
        }



        public async Task<List<INovela>> ObtenTodasNovelasAsync()
        {
            List<INovela> output = new List<INovela>();
            List<Task> tareas = new List<Task>();

            string findAllNovels = SelectAllNovel_qry();

            using IDbConnection cnn = DataBaseAccess.GetConnection();

            List<InformacionNovelaDB> novels = cnn.Query<InformacionNovelaDB>(findAllNovels).ToList();

            foreach (InformacionNovelaDB infoNov in novels)
            {
                tareas.Add(Task.Run(() =>
                {
                    EncuentraGenerosYTags(cnn, infoNov);
                    string getThemChapters = SelectCaps_qry(infoNov);
                    var Capitulos = cnn.Query<Capitulo>(getThemChapters);
                    output.Add(GetNovelsFactory.FabricaNovela(Capitulos, infoNov));
                }));
            }

            await Task.WhenAll(tareas);
            cnn.Dispose();
            return output;
        }


        #endregion



        #region Helpers


        /// <summary>
        /// Usando el Diccionario de CapitulosAGuardar, este metodo ejecutará queries a la DB mientras queden capitulos en el dicc. (Cada vez que guarda 1, lo remueve del dicc).
        /// </summary>
        private void CapitulosAGuardar_Ejecuta()
        {
            if (CapitulosAGuardar.Count < 1) return;
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            EjecutandoGuardado = true;
            while (CapitulosAGuardar.Count > 0) //Mientras hayan capitulos por guardar...
            {
                var key = CapitulosAGuardar.First();
                int NovelaID = key.Value;
                Capitulo c = key.Key;

                try
                {
                    var insertCap = InsertCap_qry(NovelaID, c);
                    cnn.Execute(insertCap);

                }
                catch (Exception ex)
                {
                    GetNovelsComunicador.ReportaError($"Metiendo texto. Saldrá un error de SQL. \nError: {ex.Message}", this);
                }

                if (string.IsNullOrEmpty(c.Texto) == false)
                {
                    string findCapID = SelectCapID_qry(c);
                    int capID = cnn.Query<int>(findCapID).First();
                    string insertTxt = InsertText_qry(c, capID);
                    cnn.Execute(insertTxt);
                }

                CapitulosAGuardar.Remove(c);
            }
            EjecutandoGuardado = false;
            cnn.Dispose();
        }


        private INovela SacaNovelaDB(Uri LinkNovela) //Debe ir privada.
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Encuentra la infobasica
            string qryNovela = SelectNovel_qry(LinkNovela);

            InformacionNovelaDB infoDBNovela = cnn.Query<InformacionNovelaDB>(qryNovela).First();

            //Encuentra los capitulos
            string qryCapitlos = SelectCaps_qry(infoDBNovela);
            List<Capitulo> Capitulos = cnn.Query<Capitulo>(qryCapitlos).ToList();
            EncuentraGenerosYTags(cnn, infoDBNovela);


            //Construye la Inovela
            cnn.Dispose();
            return GetNovelsFactory.FabricaNovela(Capitulos, infoDBNovela);
        }


        private static void EncuentraGenerosYTags(IDbConnection cnn, InformacionNovelaDB infoDBNovela)
        {
            string findTags = SelectTags_qry(infoDBNovela);
            List<string> Tags = cnn.Query<string>(findTags).ToList();

            //Encuentra los generos
            string findGeneros = SelectGeneros_qry(infoDBNovela);
            List<string> Generos = cnn.Query<string>(findGeneros).ToList();

            infoDBNovela.Generos = ManipuladorStrings.TagsEnString(Generos);
            infoDBNovela.Tags = ManipuladorStrings.TagsEnString(Tags);
        }


        private InformacionNovelaDB InsertaNovelaEnDB(InformacionNovelaOnline infoNov, IDbConnection cnn)
        {
            //1, obten el autor
            string findAuthor = SelectAutorID_qry(infoNov);
            var resultados = cnn.Query<int>(findAuthor);
            int autorID;
            if (resultados?.Any() == false)
            {
                //1.1 encuentra nacionalidad del autor en la DB.
                string findNac = SelectNacionalidadID_qry(infoNov);
                var resultadosNac = cnn.Query<int>(findNac);
                int NacID = -1;
                if (resultadosNac?.Any() == false)
                {
                    //1.2 crea nacionalidad
                    string insertNac = InsertNacionalidad_qry(infoNov);
                    cnn.Execute(insertNac);
                    NacID = cnn.Query<int>(findNac).First();
                }
                else NacID = resultadosNac.First();

                //1.2 inserta el autor
                string insertAut = InsertAutor_qry(infoNov);
                cnn.Execute(insertAut);
                autorID = cnn.Query<int>(findAuthor).First();
            }
            else autorID = resultados.First();

            //2 inserta la novela (titulo)
            string insertNov = InsertNovela_qry(infoNov, autorID);
            cnn.Execute(insertNov);
            //2.1 toma el ID de la novela
            string findNov = SelectNovelaID_qry(infoNov);
            int novID = cnn.Query<int>(findNov).First();

            //3 relacionado tags con novela
            //3.1 hallando tags id
            foreach (string tag in infoNov.Tags)
            {
                string findTag = SelectTagID_qry(tag);
                var resultadosTag = cnn.Query<int>(findTag);
                int tagID;
                if (resultadosTag?.Any() == false)
                {
                    //3.1.1 mete el tag en la DB
                    string insertTag = InsertTag_qry(tag);
                    cnn.Execute(insertTag);
                    tagID = cnn.Query<int>(insertTag).First();
                }
                else tagID = resultadosTag.First();

                //3.2 relacionando las tags con la novela
                string relacionaTags = InsertRelacionTagNovela_qry(novID, tagID);
                cnn.Execute(relacionaTags);
            }

            //4 relacionando generos con novela
            //4.1 hallando tags id
            foreach (string genero in infoNov.Generos)
            {
                string findGen = SelectGenero_qry(genero);
                var resultadosGen = cnn.Query<int>(findGen);
                int genID;
                if (resultadosGen?.Any() == false)
                {
                    //4.1.1 mete el genero en la DB
                    string insertGen = InsertGenero_qry(genero);
                    cnn.Execute(insertGen);
                    genID = cnn.Query<int>(insertGen).First();
                }
                else genID = resultadosGen.First();

                //4.2 relacionando los generos con la novela
                string relacionaGenero = InsertGenero_qry(novID, genID);
                cnn.Execute(relacionaGenero);
            }

            //5 estado de historia y traduccion
            int estadoHistoriaID = infoNov.HistoriaCompletada ? 1 : 2;
            int estadoTraduccionID = infoNov.TraduccionCompletada ? 1 : 2;
            string insertRelacionNovEstadoHistoria = InsertEstadoNovela_qry(novID, estadoHistoriaID, estadoTraduccionID);
            cnn.Execute(insertRelacionNovEstadoHistoria);

            //6 reviews
            string insertReviews = InsertReviews_qry(infoNov, novID);
            cnn.Execute(insertReviews);

            //7 Imágen
            string insertImagen = InsertImagen_qry(infoNov, novID);
            cnn.Execute(insertImagen);

            //8 Sipnosis
            string insertSipnosis = InsertSipnosis_qry(infoNov, novID);
            cnn.Execute(insertSipnosis);

            //9 Link
            string insertLink = InsertLink_qry(infoNov, novID);
            cnn.Execute(insertLink);


            InformacionNovelaDB novDBInfo = new InformacionNovelaDB()
            {
                Titulo = infoNov.Titulo,
                Autor = infoNov.Titulo,
                ID = novID,
                LinkPrincipal = infoNov.LinkPrincipal.ToString(),
                Sipnosis = infoNov.Sipnosis,
                Tags = ManipuladorStrings.TagsEnString(infoNov.Tags),
                Generos = ManipuladorStrings.TagsEnString(infoNov.Generos),
                Imagen = infoNov.Imagen.ToString()
            };

            GetNovelsEvents.Invoke_NovelaAgregadaADB();
            return novDBInfo;
        }


        #endregion


        #region Queries
        private static string InsertReviews_qry(InformacionNovelaOnline infoNov, int novID)
        {
            return $"insert into {i.TReviews} (NovelaID, Review, CantidadReviews) values ('{novID}', '{infoNov.Review}', '{infoNov.CantidadReviews}')";
        }

        private static string InsertImagen_qry(InformacionNovelaOnline infoNov, int novID)
        {
            return $"insert into {i.TImagenes} (NovelaID, Link) values ('{novID}', '{infoNov.Imagen}')";
        }

        private static string InsertSipnosis_qry(InformacionNovelaOnline infoNov, int novID)
        {
            return $"insert into {i.TSipnosis} (NovelaID, Texto) values ('{novID}', '{infoNov.Sipnosis}')";
        }

        private static string InsertLink_qry(InformacionNovelaOnline infoNov, int novID)
        {
            return $"insert into {i.TLinks} (NovelaID, Link) values ('{novID}', '{infoNov.LinkPrincipal}')";
        }

        private static string InsertEstadoNovela_qry(int novID, int estadoHistoriaID, int estadoTraduccionID)
        {
            return $"insert into {i.TEstadoNovela} (NovelaID, EstadoHistoriaID, EstadoTraduccionID) values " +
                                                                    $"('{novID}', '{estadoHistoriaID}', '{estadoTraduccionID}')";
        }

        private static string InsertGenero_qry(int novID, int genID)
        {
            return $"insert into {i.TGenerosNovela} (NoveaID, GeneroID) values ('{novID}', '{genID}')";
        }

        private static string InsertGenero_qry(string genero)
        {
            return $"insert into {i.TGeneros} Descripcion = '{genero}' ";
        }

        private static string SelectGenero_qry(string genero)
        {
            return $"select GeneroID from {i.TGeneros} where Descripcion = '{genero}' ";
        }

        private static string InsertRelacionTagNovela_qry(int novID, int tagID)
        {
            return $"insert into {i.TTagsNovelas} (NoveaID, TagID) values ('{novID}', '{tagID}')";
        }

        private static string InsertTag_qry(string tag)
        {
            return $"insert into {i.TTags} Descripcion = '{tag}' ";
        }

        private static string SelectTagID_qry(string tag)
        {
            return $"select TagID from {i.TTags} where Descripcion = '{tag}' ";
        }

        private static string SelectNovelaID_qry(InformacionNovelaOnline infoNov)
        {
            return $"select NovelaID frmo {i.TNovelas} where NovelaTitulo = '{infoNov.Titulo}' ";
        }

        private static string InsertNovela_qry(InformacionNovelaOnline infoNov, int autorID)
        {
            return $"insert into {i.TNovelas} (AutorID, NovelaTitulo) values ('{autorID}', '{infoNov.Titulo}')";
        }

        private static string InsertAutor_qry(InformacionNovelaOnline infoNov)
        {
            return $"insert into {i.TAutores} (Nacionalidad, NombreAutor) values ('{infoNov.Nacionalidad}', '{infoNov.Autor}')";
        }

        private static string InsertNacionalidad_qry(InformacionNovelaOnline infoNov)
        {
            return $"insert into {i.TNaciones} (NacionNombre) values ('{infoNov.Nacionalidad}') ";
        }

        private static string SelectNacionalidadID_qry(InformacionNovelaOnline infoNov)
        {
            return $"select NacionID from {i.TNaciones} where NacionNombre = '{infoNov.Nacionalidad}' ";
        }

        private static string SelectAutorID_qry(InformacionNovelaOnline infoNov)
        {
            return $"select AutorID from {i.TAutores} where NombreAutor = '{infoNov.Autor}'";
        }


        private static string InsertText_qry(Capitulo c, int capID)
        {
            return $"insert into {i.TTextosCapitulos} (CapituloID, Texto) values ('{capID}', \"{c.Texto}\") ";
        }

        private static string SelectCapID_qry(Capitulo c)
        {
            return $"select CapituloID from {i.TCapitulos} where Link = \"{c.Link}\" ";
        }

        private static string InsertCap_qry(int NovelaID, Capitulo c)
        {
            return $"insert into {TablaCapitulos} " +
                                                    $"(NovelaID, Link, Numero, Titulo, Valor) values" +
                                                    $"('{NovelaID}', '{c.Link}', '{c.NumeroCapitulo}', '{c.TituloCapitulo}', '{c.Valor}')";
        }

        private static string SelectNovel_qry(Uri LinkNovela)
        {
            return $"Select " +
                    "nov.NovelaTitulo as Titulo," +
                    "nov.NovelaID as ID, " +
                    "au.NombreAutor as Autor, " +
                    "nac.NacionNombre as Nacionalidad, " +
                    "T.EstadoTraduccionID as TraduccionCompleta, " +
                    "H.EstadoHistoriaID as HistoriaCompleta, " +
                    "RV.Review as Review, " +
                    "RV.CantidadReviews as CantidadReviews, " +
                    "S.Texto as Sipnosis, " +
                    "L.Link as LinkPrincipal, " +
                    "I.Link as Imagen " +
                    "from Novelas as nov " +
                    "join Autores as au " +
                    "on nov.AutorID = au.AutorID " +
                    "join Naciones as nac " +
                        "on au.AutorID = nac.NacionID " +
                    "join EstadoNovelas as est " +
                        "on nov.NovelaID = est.NovelaID " +
                    "join EstadoHistoria as H " +
                        "on est.EstadoHistoriaID = H.EstadoHistoriaID " +
                    "join EstadoTraduccion as T " +
                        "on est.EstadoTraduccionID = T.EstadoTraduccionID " +
                    "join ReviewsNovelas as RV " +
                        "on nov.NovelaID = RV.NovelaID " +
                    "join Sipnosis as S " +
                        "on S.NovelaID = nov.NovelaID " +
                    "join Links as L " +
                        $"on L.NovelaID = nov.NovelaID' and L.Link = \"{LinkNovela}\" " +
                    "join Imagenes as I " +
                        "on I.NovelaID = nov.NovelaID";
        }

        private static string SelectAllNovel_qry()
        {
            return $"Select " +
                    "nov.NovelaTitulo as Titulo," +
                    "nov.NovelaID as ID, " +
                    "au.NombreAutor as Autor, " +
                    "nac.NacionNombre as Nacionalidad, " +
                    "T.EstadoTraduccionID as TraduccionCompleta, " +
                    "H.EstadoHistoriaID as HistoriaCompleta, " +
                    "RV.Review as Review, " +
                    "RV.CantidadReviews as CantidadReviews, " +
                    "S.Texto as Sipnosis, " +
                    "L.Link as LinkPrincipal, " +
                    "I.Link as Imagen " +
                    "from Novelas as nov " +
                    "join Autores as au " +
                    "on nov.AutorID = au.AutorID " +
                    "join Naciones as nac " +
                        "on au.AutorID = nac.NacionID " +
                    "join EstadoNovelas as est " +
                        "on nov.NovelaID = est.NovelaID " +
                    "join EstadoHistoria as H " +
                        "on est.EstadoHistoriaID = H.EstadoHistoriaID " +
                    "join EstadoTraduccion as T " +
                        "on est.EstadoTraduccionID = T.EstadoTraduccionID " +
                    "join ReviewsNovelas as RV " +
                        "on nov.NovelaID = RV.NovelaID " +
                    "join Sipnosis as S " +
                        "on S.NovelaID = nov.NovelaID " +
                    "join Links as L " +
                        "on L.NovelaID = nov.NovelaID' " +
                    "join Imagenes as I " +
                        "on I.NovelaID = nov.NovelaID";
        }


        private static string SelectCaps_qry(InformacionNovelaDB infoDBNovela)
        {
            return  $"select " +
                        "c.Link as Link, " +
                        "t.Texto as Texto,  " +
                        "c.Titulo as TituloCapitulo, "+
                        "c.Numero as NumeroCapitulo, " +
                        "c.Valor as Valor " +
                    "from Capitulos as c "+
                    "left join TextosCapitulos as t " +
                        "on c.CapituloID = t.CapituloID  " +
                        $"and c.NovelaID = '{infoDBNovela.ID}'";
        }

        private static string SelectGeneros_qry(InformacionNovelaDB infoDBNovela)
        {
            return $"select " +
                   "    g.Descripcion " +
                   "from GenerosNovela as gn" +
                   "join Generos as g " +
                    "on g.GeneroID = gn.GeneroID and " +
                    $"gn.NovelaID = '{infoDBNovela.ID}'";
        }

        private static string SelectTags_qry(InformacionNovelaDB infoDBNovela)
        {
            return $"select " +
                   "t.Descripcion "+
                   "from TagsNovela as tn" +
                   "join Tags as t " +
                   "on t.TagID = tn.TagID and" +
                   $"tn.NovelaID = '{infoDBNovela.ID}' ";
        } 
        #endregion
    }




    

    internal static class i
    {
        //enumsish
       public const string TEstadoHistoria = "EstadoHistoria";
       public const string TEstadoTraduccion = "EstadoTraduccion";
       public const string TNaciones = "Naciones";
       public const string TOrdenLinks = "OrdenLinks";
       public const string TTags = "Tags";
       public const string TGeneros = "Generos";

       //novelas
       public const string TAutores = "Autores";
       public const string TEstadoNovela = "EstadoNovelas";
       public const string TLinks = "Links";
       public const string TImagenes = "Imagenes";
       public const string TNovelas = "Novelas";
       public const string TReviews = "ReviewsNovelas";
       public const string TSipnosis = "Sipnosis";
       public const string TGenerosNovela = "GenerosNovela";
       public const string TTagsNovelas = "TagsNovelas";
       
       public const string TCapitulos = "Capitulos";
       public const string TTextosCapitulos = "TextosCapitulos";
       
       //configs
       public const string TConfiguracion = "Configuracion";
       public const string TWebsites = "Websites";
       public const string TxPathsLinks = "xPathsLinks";

       public const string TxPathsTextos = "xPathsTextos";
       public const string TxPathsTitulo = "xPathsTitulo";



        static string fkme = $"Select " +
                                $@"nov.NovelaTitulo,
	                               nov.NovelaID,
	                               au.NombreAutor,
	                               nac.NacionNombre,
	                               T.DescripcionEstado as Traduccion,
	                               H.DescripcionEstado as Historia,
	                               RV.Review,
	                               RV.CantidadReviews as CantidadReviews,
	                               S.Texto as Sipnosis,
	                               L.Link as LinkPrincipal,
	                               I.Link as Imagen " +
                                $@"from Novelas as nov
                                  join Autores as au
                                    on nov.AutorID = au.AutorID
                                  join Naciones as nac
                                    on au.AutorID = nac.NacionID
                                  join EstadoNovelas as est
                                    on nov.NovelaID = est.NovelaID
                                  join EstadoHistoria as H
                                    on est.EstadoHistoriaID = H.EstadoHistoriaID
                                  join EstadoTraduccion as T
                                    on est.EstadoTraduccionID = T.EstadoTraduccionID
                                  join ReviewsNovelas as RV
                                    on nov.NovelaID = RV.NovelaID
                                  join Sipnosis as S
                                    on S.NovelaID = nov.NovelaID
                                  join Links as L
                                    on L.NovelaID = nov.NovelaID
                                  join Imagenes as I
                                    on I.NovelaID = nov.NovelaID";
    }

}
