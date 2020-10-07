using System;
using Xunit;
using HlidacStatu.Lib.Data.Graphs2;
using System.Linq;

namespace GraphTest
{
    public class GraphTests
    {
        [Fact]
        public void GraphIsBuildCorrectly()
        {
            /*  graph visual:
             *      ┌──>va1──>va2──>va3<<─┐
             *  v1──┼──>vc1─────────────┤
             *      └──>vb1──>vb2──>vb3<<─┘
             */

            string correctPathName = "correctPath";
            var graph = new UnweightedGraph<int>();
            Vertex<int> v1 = new Vertex<int>(1);
            Vertex<int> va1 = new Vertex<int>(2);
            Vertex<int> va2 = new Vertex<int>(3);
            Vertex<int> va3 = new Vertex<int>(4);
            Vertex<int> vb1 = new Vertex<int>(5);
            Vertex<int> vb2 = new Vertex<int>(6);
            Vertex<int> vb3 = new Vertex<int>(7);
            Vertex<int> vc1 = new Vertex<int>(8);
            graph.AddEdge(v1, va1, "name");
            graph.AddEdge(va1, va2, "name");
            graph.AddEdge(va2, va3, "name");
            graph.AddEdge(v1, vb1, "name");
            graph.AddEdge(vb1, vb2, "name");
            graph.AddEdge(vb2, vb3, "name");
            graph.AddEdge(v1, vc1, correctPathName);
            graph.AddEdge(vc1, va3, correctPathName);
            graph.AddEdge(vc1, vb3, "name");


            Assert.Equal(8, graph.Vertices.Count);
            Assert.Equal(9, graph.Edges.Count());

            var bfs = graph.BreathFirstIterator(vb2).ToList();
            Assert.Equal(2, bfs.Count);

            Vertex<int> vstart = new Vertex<int>(1);
            Vertex<int> vend = new Vertex<int>(4);
            var sp = graph.ShortestPath(vstart, vend).ToList();
            Assert.Equal(2, sp.Count());

            Assert.All(sp, e => Assert.True(e.BindingName == correctPathName));
            Assert.Equal(v1, sp[0].From);
            Assert.Equal(vc1, sp[0].To);
            Assert.Equal(vc1, sp[1].From);
            Assert.Equal(va3, sp[1].To);

        }
    }
}
