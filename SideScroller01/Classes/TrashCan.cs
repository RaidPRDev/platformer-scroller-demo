using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SideScroller01
{
    enum TrashCanState
    {
        Normal,
        Hit
    }
    class TrashCan : GameItem
    {

        const int BASE_DAMAGE = 200;
        static Vector2 baseHitSpeed = new Vector2(6, -6);  // Speed to use when kicked
        static float gravity = 0.3f;

        TrashCanState state;
        Texture2D texture;
        Vector2 origin;
        Vector2 speed;

        GameItem dropItem;
        /// <summary>
        /// Create a TrashCan GameItem containing an item to drop upon its desctruction
        /// </summary>
        /// <param name="inLevel"></param>
        /// <param name="startPos"></param>
        /// <param name="dropItem"></param>
        public TrashCan(Level inLevel, Vector2 startPos, GameItem dropItem)
        {
            this.InLevel = inLevel;
            this.state = TrashCanState.Normal;
            this.texture = Game1.SprTrashCanNormal;
            this.Position = startPos;
            this.speed = Vector2.Zero;
            this.origin = new Vector2(texture.Width / 2, texture.Height); // Lower Center
            this.HitArea = Game1.SprTrashCanNormal.Width / 2;

            this.GetLayerDepth(this.Position.Y);

            this.dropItem = dropItem;

        }

        /// <summary>
        /// Create a TrashCan GameItem containing an item that will NOT drop anything
        /// </summary>
        /// <param name="inLevel"></param>
        /// <param name="startPos"></param>
        public TrashCan(Level inLevel, Vector2 startPos)
        {
            this.InLevel = inLevel;
            this.state = TrashCanState.Normal;
            this.texture = Game1.SprTrashCanNormal;
            this.Position = startPos;
            this.speed = Vector2.Zero;
            this.origin = new Vector2(texture.Width / 2, texture.Height); // Lower Center
            this.HitArea = Game1.SprTrashCanNormal.Width / 2;

            this.GetLayerDepth(this.Position.Y);

            this.dropItem = null;

        }

        private void SetEffect(string effect, DirectionTarget facing, Level inLevel, TrashCan who)
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, facing, inLevel, who);
            InLevel.GameItems.Add(spark);
        }

        public override void Update(GameTime gT)
        {
            if (state == TrashCanState.Hit)
            {
                this.speed.Y += gravity; // pull down gravity   
                this.Position += speed;

                // Check for Actor collisions
                for (int i = 0; i < InLevel.Actors.Count; i++)
                {
                    if (InLevel.Actors[i] as Player == null)
                    {
                        if (this.CheckEnemyCollision(InLevel.Actors[i]))
                        {
                            // Which way we traveling
                            if (this.speed.X < 0) // Going Left
                            {
                                SetEffect("trashcanhit", DirectionTarget.Left, InLevel, this); 
                                InLevel.Actors[i].GetKnockedDown(DirectionTarget.Left, BASE_DAMAGE);
                                // If I ever want a item to go through all actors and knock them down as well
                                // We dont use the following, as its for Removing the item after the first hit it finds.
                                //InLevel.GameItems.Remove(this);
                                //return;
                            }
                            else // Going Right
                            {
                                //string fxID, DirectionTarget facing, Level inLevel, TrashCan trashCan)
                                SetEffect("trashcanhit", DirectionTarget.Right, InLevel, this); 
                                InLevel.Actors[i].GetKnockedDown(DirectionTarget.Right, BASE_DAMAGE);
                                // If I ever want a item to go through all actors and knock them down as well
                                // We dont use the following, as its for Removing the item after the first hit it finds.
                                //InLevel.GameItems.Remove(this);
                                //return;
                            }
                        }
                    }
                }

                if (speed.Y >= 6)
                {
                    RemoveTrashCan();
                }
            }
        }

        public override void Draw(SpriteBatch SB)
        {
            //  Check for facing direction based of speed
            if (this.speed.X > 0) // HEADING LEFT so hit must be from the right
                SB.Draw(this.texture, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.origin, 1f, SpriteEffects.FlipHorizontally, this.LayerDepth);
            else //  Heading to the right, hit form LEFT
                SB.Draw(this.texture, Camera.GetScreenPosition(this.Position),
                    null, Color.White, 0f, this.origin, 1f, SpriteEffects.None, this.LayerDepth);
        }

        private void RemoveTrashCan()
        {
            if (this.dropItem != null)
            {
                // Drop the item
                InLevel.GameItems.Add(this.dropItem);
                this.dropItem.SetPosition(this.Position);
                this.dropItem = null;
            }
            // Remove TrashCan from level
            InLevel.GameItems.Remove(this);
            // Or the item can be removed like so...
            //GameManager.Levels[GameManager.CurrentLevel].GameItems.Remove(this);

        }

        public override void TakeHit(DirectionTarget cameFrom)
        {
            // Set speed and texture
            this.state = TrashCanState.Hit;
            this.texture = Game1.SprTrashCanHit;

            // Set speed based off hitDirection
            if (cameFrom == DirectionTarget.Left)
                this.speed = new Vector2(-baseHitSpeed.X, baseHitSpeed.Y);
            else
                this.speed = baseHitSpeed;
            
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

    }
}
