// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Xunit;

namespace Microsoft.Extensions.Features.Tests;
public class FeatureReferencesTests
{
    [Fact]
    public void AccessingFeatureThroughCacheGetsTheFeature()
    {
        var features = new FeatureCollection();
        var currentThing = new Thing();
        features.Set<IThing>(currentThing);
        var references = new FeatureReferences<FeatureHolder>(features);

        var thing0 = references.Fetch(ref references.Cache.Thing, (f) => null);
        var thing1 = references.Fetch(ref references.Cache.Thing, (f) => null);
        var thing2 = references.Fetch(ref references.Cache.Thing, (f) => null);

        Assert.Same(currentThing, thing0);
        Assert.Same(thing0, thing1);
        Assert.Same(thing1, thing2);
    }


    [Fact]
    public void CreatingFeatureLazilyWillSetOnFeatureCollection()
    {
        var features = new FeatureCollection();
        var references = new FeatureReferences<FeatureHolder>(features);

        Assert.Null(features.Get<IThing>());

        var thing0 = references.Fetch(ref references.Cache.Thing, (f) => new Thing());
        var currentThing = features.Get<IThing>();

        Assert.Same(thing0, currentThing);
    }

    [Fact]
    public void AccessingFeatureThroughCacheGetsTheNewFeatureWhenChangedOnFeatureCollection()
    {
        var features = new FeatureCollection();
        var currentThing = new Thing();
        features.Set<IThing>(currentThing);
        var references = new FeatureReferences<FeatureHolder>(features);

        var thing0 = references.Fetch(ref references.Cache.Thing, (f) => null);

        Assert.Same(thing0, currentThing);

        var newThing = new Thing();
        features.Set<IThing>(newThing);

        var thing1 = references.Fetch(ref references.Cache.Thing, (f) => null);

        Assert.Equal(references.Revision, features.Revision);
        Assert.Same(thing1, newThing);
    }

    public struct FeatureHolder
    {
        public IThing? Thing;
    }
}
