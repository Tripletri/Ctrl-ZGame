﻿namespace RobotVoyager.Engine
{
    internal interface IPhysics
    {
        float FrictionCoefficient { get; set; }
        // bool CalculateFriction { get; set; }

        float DecelerationSpeed { get; set; }

        //void AddImpact(PointF impact);
        //PointF SlidingFrictionForce { get; }
        //float Gravity { get; }
        //float Mass { get; }
        //PointF Impact { get; }

        //void AddForce(PointF force);
    }
}