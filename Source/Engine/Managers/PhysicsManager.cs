using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Engine.Components;
using Engine.ECS.Managers;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils.MathUtils;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Engine.Managers
{
    public class PhysicsManager : Manager<PhysicsManager>
    {
        unsafe struct NarrowPhaseCallbacks : INarrowPhaseCallbacks
        {
            /// <summary>
            /// Performs any required initialization logic after the Simulation instance has been constructed.
            /// </summary>
            /// <param name="simulation">Simulation that owns these callbacks.</param>
            public void Initialize(Simulation simulation) { }

            /// <summary>
            /// Chooses whether to allow contact generation to proceed for two overlapping collidables.
            /// </summary>
            /// <param name="workerIndex">Index of the worker that identified the overlap.</param>
            /// <param name="a">Reference to the first collidable in the pair.</param>
            /// <param name="b">Reference to the second collidable in the pair.</param>
            /// <returns>True if collision detection should proceed, false otherwise.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
            {
                return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
            }

            /// <summary>
            /// Chooses whether to allow contact generation to proceed for the children of two overlapping collidables in a compound-including pair.
            /// </summary>
            /// <param name="pair">Parent pair of the two child collidables.</param>
            /// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
            /// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
            /// <returns>True if collision detection should proceed, false otherwise.</returns>
            /// <remarks>This is called for each sub-overlap in a collidable pair involving compound collidables. If neither collidable in a pair is compound, this will not be called.
            /// For compound-including pairs, if the earlier call to AllowContactGeneration returns false for owning pair, this will not be called. Note that it is possible
            /// for this function to be called twice for the same subpair if the pair has continuous collision detection enabled; 
            /// the CCD sweep test that runs before the contact generation test also asks before performing child pair tests.</remarks>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
            {
                return true;
            }

            /// <summary>
            /// Provides a notification that a manifold has been created for a pair. Offers an opportunity to change the manifold's details. 
            /// </summary>
            /// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
            /// <param name="pair">Pair of collidables that the manifold was detected between.</param>
            /// <param name="manifold">Set of contacts detected between the collidables.</param>
            /// <param name="pairMaterial">Material properties of the manifold.</param>
            /// <returns>True if a constraint should be created for the manifold, false otherwise.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : struct, IContactManifold<TManifold>
            {
                pairMaterial.FrictionCoefficient = 1f;
                pairMaterial.MaximumRecoveryVelocity = 2f;
                pairMaterial.SpringSettings = new SpringSettings(30, 1);
                return true;
            }

            /// <summary>
            /// Provides a notification that a manifold has been created between the children of two collidables in a compound-including pair.
            /// Offers an opportunity to change the manifold's details. 
            /// </summary>
            /// <param name="workerIndex">Index of the worker thread that created this manifold.</param>
            /// <param name="pair">Pair of collidables that the manifold was detected between.</param>
            /// <param name="childIndexA">Index of the child of collidable A in the pair. If collidable A is not compound, then this is always 0.</param>
            /// <param name="childIndexB">Index of the child of collidable B in the pair. If collidable B is not compound, then this is always 0.</param>
            /// <param name="manifold">Set of contacts detected between the collidables.</param>
            /// <returns>True if this manifold should be considered for constraint generation, false otherwise.</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
            {
                return true;
            }

            /// <summary>
            /// Releases any resources held by the callbacks. Called by the owning narrow phase when it is being disposed.
            /// </summary>
            public void Dispose() { }
        }
        public struct PoseIntegratorCallbacks : IPoseIntegratorCallbacks
        {
            public Vector3 Gravity;
            Vector3 gravityDt;

            /// <summary>
            /// Performs any required initialization logic after the Simulation instance has been constructed.
            /// </summary>
            /// <param name="simulation">Simulation that owns these callbacks.</param>
            public void Initialize(Simulation simulation) { }

            /// <summary>
            /// Gets how the pose integrator should handle angular velocity integration.
            /// </summary>
            public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving; //Don't care about fidelity in this demo!

            public PoseIntegratorCallbacks(Vector3 gravity) : this()
            {
                Gravity = gravity;
            }

            /// <summary>
            /// Called prior to integrating the simulation's active bodies. When used with a substepping timestepper, this could be called multiple times per frame with different time step values.
            /// </summary>
            /// <param name="dt">Current time step duration.</param>
            public void PrepareForIntegration(float dt)
            {
                gravityDt = Gravity * dt;
            }

            /// <summary>
            /// Callback called for each active body within the simulation during body integration.
            /// </summary>
            /// <param name="bodyIndex">Index of the body being visited.</param>
            /// <param name="pose">Body's current pose.</param>
            /// <param name="localInertia">Body's current local inertia.</param>
            /// <param name="workerIndex">Index of the worker thread processing this body.</param>
            /// <param name="velocity">Reference to the body's current velocity to integrate.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
            {
                if (localInertia.InverseMass > 0)
                {
                    velocity.Linear = velocity.Linear + gravityDt;
                }
            }

        }
        public class ThreadDispatcher : IThreadDispatcher, IDisposable
        {
            struct Worker
            {
                public Thread Thread;
                public AutoResetEvent Signal;
            }
            private int threadCount;

            public int ThreadCount => threadCount;

            private Worker[] workers;
            private AutoResetEvent finished;

            private BufferPool[] bufferPools;

            private volatile Action<int> workerBody;
            private volatile bool disposed;

            private int workerIndex;
            private int completedWorkerCounter;

            public ThreadDispatcher(int threadCount)
            {
                this.threadCount = threadCount;
                workers = new Worker[threadCount - 1];

                for (int i = 0; i < workers.Length; ++i)
                {
                    workers[i] = new Worker { Thread = new Thread(WorkerLoop), Signal = new AutoResetEvent(false) };
                    workers[i].Thread.IsBackground = true;
                    workers[i].Thread.Start(workers[i].Signal);
                }

                finished = new AutoResetEvent(false);
                bufferPools = new BufferPool[threadCount];

                for (int i = 0; i < bufferPools.Length; ++i)
                {
                    bufferPools[i] = new BufferPool();
                }
            }

            void DispatchThread(int workerIndex)
            {
                workerBody(workerIndex);

                if (Interlocked.Increment(ref completedWorkerCounter) == threadCount)
                {
                    finished.Set();
                }
            }

            void WorkerLoop(object untypedSignal)
            {
                var signal = (AutoResetEvent)untypedSignal;

                while (true)
                {
                    signal.WaitOne();
                    if (disposed)
                        return;
                    DispatchThread(Interlocked.Increment(ref workerIndex) - 1);
                }
            }

            void SignalThreads()
            {
                for (int i = 0; i < workers.Length; ++i)
                {
                    workers[i].Signal.Set();
                }
            }

            public void DispatchWorkers(Action<int> workerBody)
            {
                workerIndex = 1;
                completedWorkerCounter = 0;
                this.workerBody = workerBody;

                SignalThreads();

                DispatchThread(0);

                finished.WaitOne();
                this.workerBody = null;
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    SignalThreads();
                    for (int i = 0; i < bufferPools.Length; ++i)
                    {
                        bufferPools[i].Clear();
                    }
                    foreach (var worker in workers)
                    {
                        worker.Thread.Join();
                        worker.Signal.Dispose();
                    }
                }
            }

            public BufferPool GetThreadMemoryPool(int workerIndex)
            {
                return bufferPools[workerIndex];
            }
        }

        private Simulation simulation;
        private BufferPool bufferPool;
        private ThreadDispatcher threadDispatcher;

        public void Init()
        {
            bufferPool = new BufferPool();
            simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new Vector3(0, -10, 0)), new PositionLastTimestepper());
            threadDispatcher = new ThreadDispatcher(4);

            foreach (var entity in SceneManager.Instance.Entities)
            {
                var colliderComponent = entity.GetComponent<BaseColliderComponent>();
                var transformComponent = entity.GetComponent<TransformComponent>();

                var colliderType = colliderComponent.GetType();

                if (colliderType == typeof(BoxColliderComponent))
                {
                    var box = new Box(2, 2, 2);
                    box.ComputeInertia(colliderComponent.Mass, out var inertia);
                    var collidable = new CollidableDescription(simulation.Shapes.Add(box), 0.1f);

                    if (colliderComponent.IsStatic)
                    {
                        simulation.Statics.Add(new StaticDescription(
                            transformComponent.Position,
                            collidable
                        ));
                    }
                    else
                    {
                        colliderComponent.BodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(
                            new RigidPose(transformComponent.Position),
                            inertia,
                            collidable,
                            new BodyActivityDescription(0.01f)
                        ));
                    }
                }
                else if (colliderType == typeof(SphereColliderComponent))
                {
                    var sphere = new Sphere((colliderComponent as SphereColliderComponent).Radius);
                    sphere.ComputeInertia(colliderComponent.Mass, out var inertia);
                    var collidable = new CollidableDescription(simulation.Shapes.Add(sphere), 0.01f);

                    if (colliderComponent.IsStatic)
                    {
                        simulation.Statics.Add(new StaticDescription(
                            transformComponent.Position,
                            collidable
                        ));
                    }
                    else
                    {
                        colliderComponent.BodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(
                            new RigidPose(transformComponent.Position),
                            inertia,
                            collidable,
                            new BodyActivityDescription(0.0001f)
                        ));
                    }
                }
                else
                {
                    continue;
                }
            }
        }

        public void Cleanup()
        {
            simulation.Dispose();
            bufferPool.Clear();
        }

        public override void Run()
        {
            simulation.Timestep(0.016f, threadDispatcher);
            foreach (var entity in SceneManager.Instance.Entities)
            {
                var colliderComponent = entity.GetComponent<BaseColliderComponent>();
                var transformComponent = entity.GetComponent<TransformComponent>();
                if (colliderComponent.IsStatic)
                {
                    var staticRef = simulation.Statics.GetStaticReference(colliderComponent.StaticHandle);
                    staticRef.Pose.Position = transformComponent.Position;
                    // staticRef.Pose.Orientation = transformComponent.Rotation.ConvertToNumerics();
                }
                else
                {
                    var bodyRef = simulation.Bodies.GetBodyReference(colliderComponent.BodyHandle);
                    transformComponent.Position = bodyRef.Pose.Position;
                    // transformComponent.Rotation = bodyRef.Pose.Orientation;
                }
            }
        }
    }
}
