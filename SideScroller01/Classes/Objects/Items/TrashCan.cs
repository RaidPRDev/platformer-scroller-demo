using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Items
{
    enum TrashCanState
    {
        Normal,
        Hit
    }

    class TrashCan : GameItem
    {
        private const int BASE_DAMAGE = 200;
        private static Vector2 baseHitSpeed = new Vector2(6, -6);  // Speed to use when kicked
        private static float gravity = 0.3f;

        private TrashCanState State;
        private Texture2D SpriteHit;
        private GameItem BonusItem;

        /// <summary>
        /// Create a TrashCan GameItem containing an item to drop upon its desctruction
        /// </summary>
        /// <param name="inLevel"></param>
        /// <param name="startPos"></param>
        /// <param name="bonusItem"></param>
        public TrashCan(Level inLevel, Vector2 startPos, GameItem bonusItem)
            : base(startPos, inLevel, null)
        {
            this.Sprite = TextureManager.GetTexure("TrashCanNormal");
            this.SpriteHit = TextureManager.GetTexure("TrashCanHit");
            this.InitSpriteFrames(this.Sprite.Width, this.Sprite.Height);
            this.SetOriginPosition("bottom");
            this.HitArea = this.DrawWidth / 2;
            this.GetLayerDepth(this.Position.Y);
            this.BonusItem = bonusItem;

            this.State = TrashCanState.Normal;
        }

        /// <summary>
        /// Create a TrashCan GameItem with no bonus item
        /// </summary>
        /// <param name="inLevel"></param>
        /// <param name="startPos"></param>
        public TrashCan(Level inLevel, Vector2 startPos)
            : base(startPos, inLevel, null)
        {
            this.Sprite = TextureManager.GetTexure("TrashCanNormal");
            this.SpriteHit = TextureManager.GetTexure("TrashCanHit");
            this.InitSpriteFrames(this.Sprite.Width, this.Sprite.Height);
            this.SetOriginPosition("bottom");
            this.HitArea = this.DrawWidth / 2;
            this.GetLayerDepth(this.Position.Y);
            this.BonusItem = null;
            this.State = TrashCanState.Normal;
        }

        private void SetEffect(string effect, DirectionTarget facing, Level inLevel, TrashCan who)
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, facing, inLevel, who);

            // Add to level 
            InLevel.GameItems.Add(spark);
        }

        public override void Update(GameTime gT)
        {
            if (this.State == TrashCanState.Hit)
            {
                this.Speed.Y += gravity; // pull down gravity   
                this.Position += this.Speed;

                // Check for Actor collisions
                for (int i = 0; i < this.InLevel.Actors.Count; i++)
                {
                    if (this.InLevel.Actors[i] as Player == null)
                    {
                        if (this.CheckEnemyCollision(this.InLevel.Actors[i]))
                        {
                            // Which way we traveling
                            if (this.Speed.X < 0) // Going Left
                            {
                                this.SetEffect("trashcanhit", DirectionTarget.Left, this.InLevel, this);
                                this.InLevel.Actors[i].GetKnockedDown(DirectionTarget.Left, BASE_DAMAGE);
                                // If I ever want a item to go through all actors and knock them down as well
                                // We dont use the following, as its for Removing the item after the first hit it finds.
                                //InLevel.GameItems.Remove(this);
                                //return;
                            }
                            else // Going Right
                            {
                                //string fxID, DirectionTarget facing, Level inLevel, TrashCan trashCan)
                                this.SetEffect("trashcanhit", DirectionTarget.Right, this.InLevel, this);
                                this.InLevel.Actors[i].GetKnockedDown(DirectionTarget.Right, BASE_DAMAGE);
                                // If I ever want a item to go through all actors and knock them down as well
                                // We dont use the following, as its for Removing the item after the first hit it finds.
                                //InLevel.GameItems.Remove(this);
                                //return;
                            }
                        }
                    }
                }

                if (this.Speed.Y >= 6)
                {
                    RemoveTrashCan();
                }
            }
        }

        public override void Draw(SpriteBatch SB)
        {
            //  Check for facing direction based of speed
            if (this.Speed.X > 0) // HEADING LEFT so hit must be from the right
                SB.Draw(this.Sprite, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.Origin, 1f, SpriteEffects.FlipHorizontally, this.LayerDepth);
            else //  Heading to the right, hit form LEFT
                SB.Draw(this.Sprite, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.Origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        private void RemoveTrashCan()
        {
            if (this.BonusItem != null)
            {
                // Drop the item
                this.InLevel.GameItems.Add(this.BonusItem);
                this.BonusItem.SetPosition(this.Position);
                this.BonusItem = null;
            }

            // Remove TrashCan from level
            this.Destroy();

            // Optional, we can remove it from the GameManager
            //GameManager.Levels[GameManager.CurrentLevel].GameItems.Remove(this);
        }

        public override void TakeHit(DirectionTarget fromDir)
        {
            // Set speed and texture
            this.State = TrashCanState.Hit;
            this.Sprite = this.SpriteHit;

            // Set speed based off hitDirection
            if (fromDir == DirectionTarget.Left) this.Speed = new Vector2(-baseHitSpeed.X, baseHitSpeed.Y);
            else this.Speed = baseHitSpeed;
            
            SoundManager.PlaySound("CrashGlass");
            SoundManager.PlaySound("MetalSound2");
        }

        public override bool CheckCollision(Actor actor)
        {
            // 1) Is Actor/Enemy attackable?
            if (actor.IsAttackable)
            {
                // 2) Are wer within Y Range
                if (actor.Position.Y > this.Position.Y - Actor.HIT_Y_RANGE
                    && actor.Position.Y < this.Position.Y + Actor.HIT_Y_RANGE)
                {
                    // 3) Which way is the actor facing
                    if (actor.FacingDir == DirectionTarget.Left)
                    {
                        // 4) Is this item in front of actor
                        if (this.Position.X < actor.Position.X)
                        {
                            // 5) Is Actor's left edge <MORE LEFT> than my RIGHT edge
                            if (actor.Position.X - actor.HitArea < this.Position.X + this.HitArea)
                            {
                                
                                // There is a collision
                                return true;

                            }
                        }

                    }
                    //  3) Which way is the actor is facing
                    else // Actor facing to the LEFT
                    {
                        // A) Am I in front of Actor in in front of us or RIGHT OF the player
                        if (this.Position.X > actor.Position.X)
                        {
                            // 5) Is the actor's RIGHT EDGE is more right than my(gameitem) LEFT 
                            if (actor.Position.X + actor.HitArea > this.Position.X - this.HitArea)
                            {
                                // There is a collision
                                return true;
                            }

                        }
                    }
                }
            }

            // No Collision
            return false;
        }

        public override void Destroy()
        {
            SpriteHit = null;      // lose reference just in case

            base.Destroy();
        }
    }
}
