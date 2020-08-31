using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Util
{
    public class ExpressionParameterReplacer : ExpressionVisitor
    {
        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters)
        {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
        }
        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements
        {
            get;
            set;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            ParameterExpression replacement;
            if (ParameterReplacements.TryGetValue(node, out replacement))
                node = replacement;
            return base.VisitParameter(node);
        }
    }

    public static class Extensions
    {

        public static Expression<Func<T, bool>> AndAll<T>(params Expression<Func<T, bool>>[] expressions)
        {

            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }
            if (expressions.Count() == 0)
            {
                return t => true;
            }
            Type delegateType = typeof(Func<,>)
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(new[] {
                                typeof(T),
                                typeof(bool)
                                    }
                                );
            var exprA = expressions[0];
            var exprB = expressions[1];
            Expression<Func<T, bool>> exprC =
                    Expression.Lambda<Func<T, bool>>(
                        Expression.AndAlso(
                            exprA.Body,
                            new ExpressionParameterReplacer(exprB.Parameters, exprA.Parameters).Visit(exprB.Body)),
                        exprA.Parameters);

            //var combined = expressions
            //                   .Cast<Expression>()
            //                   .Aggregate((e1, e2) => Expression.AndAlso(e1, e2));
            //return (Expression<Func<T, bool>>)Expression.Lambda(delegateType, exprC);
            return exprC;
        }

        public static IEnumerable<TResult> FullOuterGroupJoin<TA, TB, TKey, TResult>(
     this IEnumerable<TA> a,
     IEnumerable<TB> b,
     Func<TA, TKey> selectKeyA,
     Func<TB, TKey> selectKeyB,
     Func<IEnumerable<TA>, IEnumerable<TB>, TKey, TResult> projection,
     IEqualityComparer<TKey> cmp = null)
        {
            cmp = cmp ?? EqualityComparer<TKey>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TKey>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       let xa = alookup[key]
                       let xb = blookup[key]
                       select projection(xa, xb, key);

            return join;
        }

        public static IEnumerable<TResult> FullOuterJoin<TA, TB, TKey, TResult>(
            this IEnumerable<TA> a,
            IEnumerable<TB> b,
            Func<TA, TKey> selectKeyA,
            Func<TB, TKey> selectKeyB,
            Func<TA, TB, TKey, TResult> projection,
            TA defaultA = default(TA),
            TB defaultB = default(TB),
            IEqualityComparer<TKey> cmp = null
        )
        {
            cmp = cmp ?? EqualityComparer<TKey>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TKey>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy<T, int>((item) => Consts.Rnd.Next());
        }

        /// <summary>
        /// Picks "Random" element from IEnumberable based on the day.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T TipOfTheDay<T>(this IEnumerable<T> source)
        {
            var begginingOfTheTime = new DateTime(1972, 10, 19);
            var dayDiff = DateTime.Now - begginingOfTheTime;
            int index = dayDiff.Days % source.Count();

            return source.ElementAt(index);
        }

        public static string RenderInfoFacts(this IEnumerable<InfoFact> infofacts, int number,
            bool takeSummary = true, bool shuffle = false,
            string delimiterBetween = " ",
            string lineFormat = "{0}", bool html = false)
        {
            return InfoFact.RenderInfoFacts(infofacts.ToArray(), number, takeSummary, shuffle, delimiterBetween, lineFormat, html);
        }
    }
}
