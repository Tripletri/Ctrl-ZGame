using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CtrlZ
{
    class Level
    {
        public string Name { get; set; }

        public readonly List<Transform> Objects = new List<Transform>();
        private readonly IReadOnlyCollection<ICollider> colliders;
        private Animator animator;
        
        public Player Player { get; }

        public Level(IList<Transform> objects, Player player, Animator animator)
        {
            Objects.AddRange(objects);
            Objects.Add(player);
            Player = player;
            colliders = new ReadOnlyCollection<ICollider>(Objects.OfType<ICollider>().ToList());
            this.animator = animator;
        }

        public void Update()
        {
            animator.UpdateAnimation();
            foreach (var collider in colliders)
            {
                collider.MovePhysics(colliders);
                collider.Move(colliders);
            }
        }
    }
}