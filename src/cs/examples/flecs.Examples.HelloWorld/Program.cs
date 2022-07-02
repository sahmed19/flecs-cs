﻿using System.Runtime.InteropServices;
using Flecs;

internal static class Program
{
    [StructLayout(LayoutKind.Sequential)]
    struct Position : IComponent
    {
        public double X;
        public double Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct Velocity : IComponent
    {
        public double X;
        public double Y;
    }
    
    struct Eats : ITag
    {
    }

    struct Apples : ITag
    {
    }

    struct Pears : ITag
    {
    }
    
    // Move system implementation. System callbacks may be called multiple times, as entities are grouped by which
    // components they have, and each group has its own set of component arrays.
    static void Move(SystemIterator iterator)
    {
        var p = iterator.Term<Position>(1);
        var v = iterator.Term<Velocity>(2);

        // Print the set of components for the iterated over entities
        var tableString = iterator.Table().String();
        Console.WriteLine("Move entities with table: " + tableString);

        // Iterate entities for the current group 
        for (var i = 0; i < iterator.Count; i++)
        {
            ref var position = ref p[i];
            var velocity = v[i];

            position.X += velocity.X;
            position.Y += velocity.Y;
        }
    }

    static int Main(string[] args)
    {
        // Create the world
        var world = new World(args);

        // Register components
        world.InitializeComponent<Position>();
        world.InitializeComponent<Velocity>();
        
        // Register system
        world.InitializeSystem<Position, Velocity>(Move);

        // Register tags (components without a size)
        world.InitializeTag<Eats>();
        world.InitializeTag<Apples>();
        world.InitializeTag<Pears>();

        // Create an entity with name Bob, add Position and food preference
        var bob = world.InitializeEntity("Bob");
        bob.SetComponent(new Position { X = 0, Y = 0 });
        bob.SetComponent(new Velocity { X = 2, Y = 2 });
        bob.AddPair<Eats, Apples>();

        // Run systems twice. Usually this function is called once per frame
        world.Progress(0);
        world.Progress(0);
        
        // See if Bob has moved (he has)
        var p = bob.GetComponent<Position>();
        Console.WriteLine("Bob's position is {" + p.X + ", " + p.Y + "}");

        return world.Fini();
    }
}