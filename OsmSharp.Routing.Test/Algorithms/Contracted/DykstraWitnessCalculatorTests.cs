﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using NUnit.Framework;
using OsmSharp.Routing.Algorithms.Contracted;
using OsmSharp.Routing.Algorithms.Contracted.Witness;
using OsmSharp.Routing.Data.Contracted;
using OsmSharp.Routing.Graphs.Directed;
using System.Collections.Generic;

namespace OsmSharp.Routing.Test.Algorithms.Contracted
{
    /// <summary>
    /// Contains tests for the dykstra witness calculator.
    /// </summary>
    [TestFixture]
    public class DykstraWitnessCalculatorTests
    {
        /// <summary>
        /// Test on one edge with one hop.
        /// </summary>
        [Test]
        public void TestOneEdgeOneHop()
        {
            // build graph.
            var graph = new DirectedGraph(ContractedEdgeDataSerializer.Size);
            graph.AddEdge(0, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = null,
                Weight = 100
            }));

            var witnessCalculator = new DykstraWitnessCalculator(1);

            // calculate witness for weight of 200.
            var forwardWitnesses = new bool[1];
            var backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 1 }), new List<float>(new float[] { 200 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);
            Assert.AreEqual(true, forwardWitnesses[0]);
            Assert.AreEqual(true, backwardWitnesses[0]);

            // calculate witness for weight of 50.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 1 }), new List<float>(new float[] { 50 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);

            Assert.AreEqual(false, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);
        }

        /// <summary>
        /// Test on two edges with two hops.
        /// </summary>
        [Test]
        public void TestTwoEdgeInfiniteHops()
        {
            // build graph.
            var graph = new DirectedGraph(ContractedEdgeDataSerializer.Size);
            graph.AddEdge(0, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = null,
                Weight = 100
            }));
            graph.AddEdge(1, 2, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = null,
                Weight = 100
            }));

            var witnessCalculator = new DykstraWitnessCalculator(int.MaxValue);

            // calculate witness for weight of 200.
            var forwardWitnesses = new bool[1];
            var backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 1000 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);
            Assert.AreEqual(true, forwardWitnesses[0]);
            Assert.AreEqual(true, backwardWitnesses[0]);

            // calculate witness for weight of 50.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 50 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);

            Assert.AreEqual(false, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);
        }

        /// <summary>
        /// Test on two oneway edges with two hops.
        /// </summary>
        [Test]
        public void TestTwoOnewayEdgeInfiniteHops()
        {
            // build graph.
            var graph = new DirectedGraph(ContractedEdgeDataSerializer.Size);
            graph.AddEdge(0, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 100
            }));
            graph.AddEdge(1, 2, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 100
            }));
            graph.AddEdge(0, 2, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 300
            }));

            var witnessCalculator = new DykstraWitnessCalculator(int.MaxValue);

            // calculate witness for weight of 200.
            var forwardWitnesses = new bool[1];
            var backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 1000 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);
            Assert.AreEqual(true, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);

            // calculate witness for weight of 50.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 50 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);

            Assert.AreEqual(false, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);

            // build graph.
            graph = new DirectedGraph(ContractedEdgeDataSerializer.Size);
            graph.AddEdge(1, 0, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 100
            }));
            graph.AddEdge(2, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 100
            }));
            graph.AddEdge(2, 0, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 300
            }));


            // calculate witness for weight of 200.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 2, new List<uint>(new uint[] { 0 }), new List<float>(new float[] { 1000 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);
            Assert.AreEqual(true, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);

            // calculate witness for weight of 50.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 2, new List<uint>(new uint[] { 0 }), new List<float>(new float[] { 50 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);

            Assert.AreEqual(false, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);
        }

        /// <summary>
        /// Tests calculating witnesses in a quadrilateral.
        /// </summary>
        [Test]
        public void TestQuadrilateralOneWay()
        {
            // build graph.
            var graph = new DirectedGraph(ContractedEdgeDataSerializer.Size);
            graph.AddEdge(0, 2, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 100
            }));
            graph.AddEdge(2, 0, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = false,
                Weight = 100
            }));
            graph.AddEdge(0, 3, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = false,
                Weight = 10
            }));
            graph.AddEdge(3, 0, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 10
            }));
            graph.AddEdge(1, 2, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = false,
                Weight = 1000
            }));
            graph.AddEdge(2, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 1000
            }));
            graph.AddEdge(1, 3, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = true,
                Weight = 10000
            }));
            graph.AddEdge(3, 1, ContractedEdgeDataSerializer.Serialize(new ContractedEdgeData()
            {
                ContractedId = Constants.NO_VERTEX,
                Direction = false,
                Weight = 10000
            }));
            graph.Compress(false);

            var witnessCalculator = new DykstraWitnessCalculator(int.MaxValue);

            // calculate witness for weight of 200.
            var forwardWitnesses = new bool[1];
            var backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 1000 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);
            Assert.AreEqual(true, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);

            // calculate witness for weight of 50.
            forwardWitnesses = new bool[1];
            backwardWitnesses = new bool[1];
            witnessCalculator.Calculate(graph, 0, new List<uint>(new uint[] { 2 }), new List<float>(new float[] { 50 }),
                ref forwardWitnesses, ref backwardWitnesses, uint.MaxValue);

            Assert.AreEqual(false, forwardWitnesses[0]);
            Assert.AreEqual(false, backwardWitnesses[0]);
        }
    }
}