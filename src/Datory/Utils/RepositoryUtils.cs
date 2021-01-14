using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Datory.Caching;
using SqlKata;

[assembly: InternalsVisibleTo("Datory.Data.Tests")]

namespace Datory.Utils
{
    internal static partial class RepositoryUtils
    {
        public static Query NewQuery(string tableName, Query query = null)
        {
            return query == null ? new Query(tableName) : query.Clone().From(tableName);
        }

        private static async Task<CompileInfo> CompileAsync(IDatabase database, string tableName, IRedis redis, Query query)
        {
            var method = query.Method;
            if (method == "update")
            {
                query.Method = "select";
            }

            string sql;
            Dictionary<string, object> namedBindings;

            var compiler = DbUtils.GetCompiler(database.DatabaseType, database.ConnectionString);
            var compiled = compiler.Compile(query);

            if (method == "update")
            {
                var bindings = new List<object>();

                var setList = new List<string>();
                var components = query.GetComponents("update");
                components.Add(new BasicCondition
                {
                    Column = nameof(Entity.LastModifiedDate),
                    Value = DateTime.Now
                });
                foreach (var clause in components)
                {
                    if (clause is RawCondition raw)
                    {
                        var set = compiler.WrapIdentifiers(raw.Expression);
                        if (setList.Contains(set, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        setList.Add(set);
                        if (raw.Bindings != null)
                        {
                            bindings.AddRange(raw.Bindings);
                        }
                    }
                    else if (clause is BasicCondition basic)
                    {
                        var set = compiler.Wrap(basic.Column) + " = ?";
                        if (setList.Contains(set, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        setList.Add(set);
                        bindings.Add(basic.Value);
                    }
                }
                bindings.AddRange(compiled.Bindings);

                var result = new SqlResult
                {
                    Query = query
                };
                var where = compiler.CompileWheres(result);
                sql = $"UPDATE {database.GetQuotedIdentifier(tableName)} SET { string.Join(", ", setList)} {where}";

                //sql = Helper.ExpandParameters(sql, "?", bindings.ToArray());
                sql = Helper.ReplaceAll(sql, "?", i => "@p" + i);

                namedBindings = Helper.Flatten(bindings).Select((v, i) => new { i, v })
                    .ToDictionary(x => "@p" + x.i, x => x.v);
            }
            else
            {
                sql = compiled.Sql;
                namedBindings = compiled.NamedBindings;
            }

            var compileInfo = new CompileInfo
            {
                Sql = sql,
                NamedBindings = namedBindings
            };

            var cacheList = query.GetComponents<CachingCondition>("cache");
            if (cacheList != null && cacheList.Count > 0)
            {
                var cacheManager = await CachingUtils.GetCacheManagerAsync(redis);
                foreach (var caching in cacheList)
                {
                    if (caching.Action == CachingAction.Remove && caching.CacheKeysToRemove != null)
                    {
                        foreach (var cacheKey in caching.CacheKeysToRemove)
                        {
                            cacheManager.Remove(cacheKey);
                        }
                    }
                    else if (caching.Action == CachingAction.Get)
                    {
                        compileInfo.Caching = caching;
                    }
                }
            }

            return compileInfo;
        }
    }
}
