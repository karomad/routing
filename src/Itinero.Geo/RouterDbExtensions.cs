﻿using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Itinero.Data.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Itinero.Attributes;
using Itinero.Geo.Attributes;

namespace Itinero.Geo
{
    /// <summary>
    /// Contains extensions for the router db.
    /// </summary>
    public static class RouterDbExtensions
    {
        /// <summary>
        /// Gets all features.
        /// </summary>
        public static FeatureCollection GetFeatures(this RouterDb db)
        {
            var network = db.Network;
            var features = new FeatureCollection();

            var edges = new HashSet<long>();

            var edgeEnumerator = network.GetEdgeEnumerator();
            for (uint vertex = 0; vertex < network.VertexCount; vertex++)
            {
                var vertexLocation = network.GeometricGraph.GetVertex(vertex);
                var attributes = new AttributesTable();
                attributes.AddAttribute("id", vertex.ToInvariantString());
                features.Add(new Feature(new Point(vertexLocation.ToCoordinate()),
                    attributes));
                edgeEnumerator.MoveTo(vertex);
                edgeEnumerator.Reset();
                while (edgeEnumerator.MoveNext())
                {
                    if (edges.Contains(edgeEnumerator.Id))
                    {
                        continue;
                    }
                    edges.Add(edgeEnumerator.Id);

                    var edgeAttributes = new Itinero.Attributes.AttributeCollection(db.EdgeMeta.Get(edgeEnumerator.Data.MetaId));
                    edgeAttributes.AddOrReplace(db.EdgeProfiles.Get(edgeEnumerator.Data.Profile));

                    var geometry = new LineString(network.GetShape(edgeEnumerator.Current).ToCoordinatesArray());
                    attributes = edgeAttributes.ToAttributesTable();
                    attributes.AddAttribute("id", edgeEnumerator.Id.ToInvariantString());
                    attributes.AddAttribute("distance", edgeEnumerator.Data.Distance.ToInvariantString());
                    var tags = db.GetProfileAndMeta(edgeEnumerator.Data.Profile, edgeEnumerator.Data.MetaId);
                    features.Add(new Feature(geometry,
                        attributes));
                }
            }

            return features;
        }

        /// <summary>
        /// Gets all features inside the given bounding box.
        /// </summary>
        public static FeatureCollection GetFeaturesIn(this RouterDb db, float minLatitude, float minLongitude,
            float maxLatitude, float maxLongitude, bool includeEdges = true)
        {
            var features = new FeatureCollection();

            var vertices = Itinero.Algorithms.Search.Hilbert.HilbertExtensions.Search(db.Network.GeometricGraph, minLatitude, minLongitude,
                maxLatitude, maxLongitude);
            var edges = new HashSet<long>();

            var edgeEnumerator = db.Network.GetEdgeEnumerator();
            foreach (var vertex in vertices)
            {
                features.Add(db.GetFeatureForVertex(vertex));

                if (includeEdges)
                {
                    edgeEnumerator.MoveTo(vertex);
                    edgeEnumerator.Reset();
                    while (edgeEnumerator.MoveNext())
                    {
                        if (edges.Contains(edgeEnumerator.Id))
                        {
                            continue;
                        }
                        edges.Add(edgeEnumerator.Id);

                        var edgeAttributes = new Itinero.Attributes.AttributeCollection(db.EdgeMeta.Get(edgeEnumerator.Data.MetaId));
                        edgeAttributes.AddOrReplace(db.EdgeProfiles.Get(edgeEnumerator.Data.Profile));

                        var geometry = new LineString(db.Network.GetShape(edgeEnumerator.Current).ToCoordinatesArray());
                        var attributes = edgeAttributes.ToAttributesTable();
                        attributes.AddAttribute("id", edgeEnumerator.Id.ToInvariantString());
                        attributes.AddAttribute("distance", edgeEnumerator.Data.Distance.ToInvariantString());
                        features.Add(new Feature(geometry,
                            attributes));
                    }
                }
            }

            return features;
        }

        /// <summary>
        /// Gets a feature representing the edge with the given id.
        /// </summary>
        public static Feature GetFeatureForEdge(this RouterDb routerDb, uint edgeId)
        {
            var edge = routerDb.Network.GetEdge(edgeId);

            var edgeAttributes = new Itinero.Attributes.AttributeCollection(routerDb.EdgeMeta.Get(edge.Data.MetaId));
            edgeAttributes.AddOrReplace(routerDb.EdgeProfiles.Get(edge.Data.Profile));

            var geometry = new LineString(routerDb.Network.GetShape(edge).ToCoordinatesArray());
            var attributes = edgeAttributes.ToAttributesTable();
            attributes.AddAttribute("id", edge.Id.ToInvariantString());
            attributes.AddAttribute("distance", edge.Data.Distance.ToInvariantString());
            return new Feature(geometry, attributes);
        }

        /// <summary>
        /// Gets a features representing the vertex with the given id.
        /// </summary>
        public static Feature GetFeatureForVertex(this RouterDb routerDb, uint vertex)
        {
            var coordinate = routerDb.Network.GetVertex(vertex).ToCoordinate();

            var attributes = new AttributesTable();
            attributes.AddAttribute("id", vertex);
            return new Feature(new Point(coordinate), attributes);
        }
    }
}