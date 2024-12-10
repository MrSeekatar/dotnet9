namespace BoxServer.Repositories;

using Dommel;

public class GuidSqlServerSqlBuilder : SqlServerSqlBuilder
{
    // works on SQLEdge on Mac, but not on SS 2022 on Linux
    public override string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames)
    {
        return $"set nocount on; insert into {tableName} ({string.Join(", ", columnNames)}) output inserted.BoxId values ({string.Join(", ", paramNames)})";
    }
}