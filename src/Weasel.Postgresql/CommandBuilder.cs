using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using Weasel.Core;

#nullable enable

namespace Weasel.Postgresql
{
    public class CommandBuilder : CommandBuilderBase<NpgsqlCommand, NpgsqlParameter, NpgsqlConnection, NpgsqlTransaction, NpgsqlDbType, NpgsqlDataReader>
    {
        public CommandBuilder() : this(new NpgsqlCommand())
        {
        }

        public CommandBuilder(NpgsqlCommand command) : base(PostgresqlProvider.Instance, ':', command)
        {
        }

        /// <summary>
        ///  Append a parameter with the supplied value to the underlying command
        /// parameter collection *and* the command text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void AppendParameter(int value)
        {
            AppendParameter(value, NpgsqlDbType.Integer);
        }

        /// <summary>
        /// Append a parameter with the supplied value to the underlying command
        /// parameter collection *and* the command text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void AppendParameter(Guid value)
        {
            AppendParameter(value, NpgsqlDbType.Uuid);
        }

        /// <summary>
        ///     Append a parameter with the supplied value to the underlying command
        /// parameter collection *and* the command text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void AppendParameter(object value, NpgsqlDbType? dbType = null)
        {
            var parameter = AddParameter(value, dbType);
            Append(_parameterPrefix);
            Append(parameter.ParameterName);
        }

        /// <summary>
        ///     Append a parameter with the supplied value to the underlying command parameter
        ///     collection and adds the parameter usage to the SQL
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void AppendParameter(string value)
        {
            AppendParameter(value, NpgsqlDbType.Varchar);
        }

        /// <summary>
        ///     Append a parameter with the supplied value to the underlying command parameter
        ///     collection and adds the parameter usage to the SQL
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        public void AppendParameter(string[] values)
        {
            AppendParameter(values, NpgsqlDbType.Varchar | NpgsqlDbType.Array);
        }

        public override string ToString()
        {
            return _sql.ToString();
        }

        /// <summary>
        /// For each public property of the parameters object, adds a new parameter
        /// to the command with the name of the property and the current value of the property
        /// on the parameters object. Does *not* affect the command text
        /// </summary>
        /// <param name="parameters"></param>
        public void AddParameters(object parameters)
        {
            if (parameters == null)
            {
                return;
            }

            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                _command.AddNamedParameter(property.Name, value);
            }
        }

        /// <summary>
        ///     Adds a parameter to the underlying command, but does NOT add the
        ///     parameter usage to the command text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddParameter(object value, NpgsqlDbType? dbType = null)
        {
            return _command.AddParameter(value, dbType);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, object value, NpgsqlDbType? dbType = null)
        {
            return _command.AddNamedParameter(name, value, dbType);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, string value)
        {
            return AddNamedParameter(name, value, NpgsqlDbType.Varchar);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, bool value)
        {
            return AddNamedParameter(name, value, NpgsqlDbType.Boolean);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, int value)
        {
            return AddNamedParameter(name, value, NpgsqlDbType.Integer);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, double value)
        {
            return AddNamedParameter(name, value, NpgsqlDbType.Double);
        }

        /// <summary>
        ///     Finds or adds a new parameter with the specified name and returns the parameter
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public NpgsqlParameter AddNamedParameter(string name, long value)
        {
            return AddNamedParameter(name, value, NpgsqlDbType.Bigint);
        }

        public NpgsqlParameter[] AppendWithParameters(string text)
        {
            var split = text.Split('?');
            var parameters = new NpgsqlParameter[split.Length - 1];

            _sql.Append(split[0]);
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = _command.AddParameter(DBNull.Value);
                parameters[i] = parameter;
                _sql.Append(_parameterPrefix);
                _sql.Append(parameter.ParameterName);
                _sql.Append(split[i + 1]);
            }

            return parameters;
        }






    }
}