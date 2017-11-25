using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Sml
{
    public static class SqlCommanderExtension
    {
        public static string ExecuteToJson(this IDbConnection cnn, string querySql, bool isStoredProcedure = false, object param = null)
        {
            var command = cnn.CreateCommand();
            command.CommandText = querySql;

            if (isStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;
            if (param != null)
                command.GenerateParameters(param);

            cnn.Open();
            var result = "{}";
            using (var reader = command.ExecuteReader())
                result = SerializeJson(reader);

            cnn.Close();
            return result;
        }


        public static List<Dictionary<string, object>> ExecuteToObject(this IDbConnection cnn, string querySql, bool isStoredProcedure, object param = null)
        {
            var command = cnn.CreateCommand();
            command.CommandText = querySql;

            if (isStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;
            if (param != null)
                command.GenerateParameters(param);

            cnn.Open();
            List<Dictionary<string, object>> result;
            using (var reader = command.ExecuteReader())
                result = Json(reader);

            cnn.Close();

            return result;
        }



        public static IEnumerable<T> ExecuteToObject<T>(this IDbConnection cnn, string querySql, bool isStoredProcedure = false, object param = null)
        {
            var command = cnn.CreateCommand();
            command.CommandText = querySql;

            if (isStoredProcedure)
                command.CommandType = CommandType.StoredProcedure;
            if (param != null)
                command.GenerateParameters(param);

            cnn.Open();
            IEnumerable<T> result;
            using (var reader = command.ExecuteReader())
                result = DataReaderMapToList<T>(reader);

            cnn.Close();
            return result;
        }


        public static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            var list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                        prop.SetValue(obj, prop.cast(dr[prop.Name].ToString()), null);
                }
                list.Add(obj);
            }
            return list;
        }




        private static object cast(this PropertyInfo prop, string value)
        {

            switch (prop.PropertyType.Name)
            {
                case "Int32":
                    return int.Parse(value);
            }
            return value;
        }


        private static void GenerateParameters(this IDbCommand command, object param)
        {
            var parameters = param.GetType().GetProperties().ToList();
            parameters.ForEach(x =>
            {
                var parameter = command.CreateParameter();
                var value = x.GetValue(param);
                parameter.ParameterName = $"@{x.Name}";
                parameter.Value = x.GetValue(param);
                parameter.DbType = Cast(value);
                command.Parameters.Add(parameter);
            });
            return;
        }

        private static DbType Cast(object obj)
        {
            switch (obj.GetType().Name)
            {
                case "Int32":
                    return DbType.Int32;
                case "Int64":
                    return DbType.Int64;
                case "String":
                case "Char":
                    return DbType.String;
                case "Boolean":
                    return DbType.Boolean;
                default:
                    return DbType.Object;
            }
        }

        private static string SerializeJson(IDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();

            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));

            var result = JsonConvert.SerializeObject(results, Formatting.None);

            return result;
        }


        private static List<Dictionary<string, object>> Json(IDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();

            for (var i = 0; i < reader.FieldCount; i++)
                cols.Add(reader.GetName(i));

            while (reader.Read())
                results.Add(SerializeRow(cols, reader));


            return results;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, IDataReader reader)
        {
            var result = new Dictionary<string, object>();
            cols.ToList().ForEach(col =>
            {
                result.Add(col, reader[col]);
            });
            return result;
        }

        private static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
                yield return projection(reader);
        }
    }
}
