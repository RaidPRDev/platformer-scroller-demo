using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Items.Base
{
    class ActorSpawner : GameItem
    {
        public ActorSpawner(Vector2 position, Level inLevel, Actor spawnedActor)
           : base(position, inLevel, spawnedActor)
        {
            
        }

        protected virtual void SpawnActor()
        {

        }

        public override void Draw(SpriteBatch SB)
        {
            SB.Draw(this.Sprite, Camera.GetScreenPosition(this.Position), this.DrawArea, Color.White, 0f, this.Origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        public override void Destroy()
        {
            this.InLevel.EnemySpawners.Remove(this); // Remove Item

            this.Sprite = null;
        }
    }
}
