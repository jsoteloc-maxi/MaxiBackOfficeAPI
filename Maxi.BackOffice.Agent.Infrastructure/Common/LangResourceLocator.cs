using Microsoft.Data.SqlClient;
using System.Text;
using Maxi.BackOffice.CrossCutting.Common.Extensions;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using SqlKata.Execution;
using Dapper;

namespace Maxi.BackOffice.Agent.Infrastructure.Common
{
    internal class LangResourceLocator : IDisposable
    {
        private readonly ApplicationContext db;

        public LangResourceLocator(ApplicationContext dbctx)
        {
            this.db = dbctx;
        }


        //Obtener Id de lenguaje segun culture
        private int GetIdLangByCulture(string aCulture)
        {
            var c = db.Query("dbo.Lenguage").Where("Culture", aCulture).FirstOrDefault();
            return c.IdLenguage ?? 0;
        }

        private string SqlSelectMessageKey()
        {
            var sql = new StringBuilder();
            sql.Append("SELECT LR.IdLenguage, LR.MessageKey, LR.Message, LN.Culture ");
            sql.Append("FROM dbo.LenguageResource LR WITH(NOLOCK) ");
            sql.Append("LEFT JOIN dbo.Lenguage LN WITH(NOLOCK) ON LN.IdLenguage = LR.IdLenguage ");
            sql.Append("WHERE LR.MessageKey = @Key ");
            sql.Append("ORDER BY 1 ");
            return sql.ToString();
        }


        string i18n_Message__ejemplo(string culture, string aKey, string aDef = "")
        {
            //Ejemplo usando directamente SqlCommand

            string c = "";
            string s = "";
            if (culture == null) culture = "";
            var cmd = new SqlCommand(SqlSelectMessageKey(), db.GetConnection(), db.Tran);
            cmd.Parameters.AddWithValue("@Key", aKey);

            var reader = cmd.ExecuteReader();

            var fieldCul = reader.GetOrdinal("Culture");
            var fieldMsg = reader.GetOrdinal("Message");

            if (reader.Read())
            {
                if (!reader.IsDBNull(fieldMsg))
                    s = reader.GetString(fieldMsg);

                c = reader.GetString(fieldCul);

                if (c.ToUpper() != culture.ToUpper())
                    while (reader.Read())
                    {
                        c = reader.GetString(fieldCul);
                        if (!reader.IsDBNull(fieldMsg) && c.ToUpper() == culture.ToUpper())
                        {
                            s = reader.GetString(fieldMsg);
                            break;
                        }
                    }
            }
            reader.Close();

            if (string.IsNullOrEmpty(s)) s = aDef;
            if (string.IsNullOrEmpty(s)) s = aKey;

            return s;
        }


        public string GetMessage(string msgKey, int lang, string aDef = "")
        {
            return GetMessage(msgKey, lang, "", aDef);
        }

        public string GetMessage(string msgKey, string culture, string aDef = "")
        {
            return GetMessage(msgKey, 0, culture, aDef);
        }

        private string GetMessage(string msgKey, int lang, string culture, string aDef = "")
        {
            string c;
            string s = "";
            string sFirst = "";
            culture = culture.IsBlank() ? "en-US" : culture;
            
            var items = db.GetConnection().Query(SqlSelectMessageKey(), new { Key = msgKey }, db.GetTransaction()).ToList();

            foreach (var t in items)
            {
                c = t.Culture ?? "";

                if (sFirst == "")
                    sFirst = t.Message ?? "";

                if (lang > 0)
                {
                    if (lang == t.IdLenguage)
                    {
                        s = t.Message ?? "";
                        break;
                    }
                }
                else if (c.EqualText(culture))
                {
                    s = t.Message ?? "";
                    break;
                }
            }//for

            if (s.IsBlank()) s = sFirst;
            if (s.IsBlank()) s = aDef;
            if (s.IsBlank()) s = msgKey;

            return s;
        }

        public void Dispose()
        {
            //
        }
    }
}
