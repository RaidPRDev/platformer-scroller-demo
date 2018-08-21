using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects.Items;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Enemies
{
    enum DeejayCloseState
    {
        // Take Hit Cycle
        TakeHit,
        TakeHitLowKick,
        KnockedDown,
        Down,
        GettingUp,
        Dying,

        // AI
        Retreat,
        MoveTo,
        PreAttack,
        Attack,
        Wait,
        LayingDown
    }

    class DeejayEnemy : Actor
    {

        #region constants

        const int HEALTH_BAR_WIDTH = 60;
        const int HEALTH_BAR_HEIGHT = 15;
        const float STARTING_HEALTH = 200;
        const float ACTOR_X_SPEED = 2f;
        const float ACTOR_Y_SPEED = 2f;
        const float ACTOR_X_RETREAT_SPEED = 0f;
        const float ACTOR_Y_RETREAT_SPEED = 0f;

        #endregion

        #region draw area constants

        // WAIT - Not doing anything, show this one
        private const int DRAW_HEIGHT_WAIT = 122;
        private const int DRAW_WIDTH_WAIT = 175;
        private const int DRAW_HEIGHT_WAIT2 = 122;
        private const int DRAW_WIDTH_WAIT2 = 175;

        private const int DRAW_HEIGHT_NORMAL = 122;
        private const int DRAW_WIDTH_IDLE = 90;
        private const int DRAW_WIDTH_WALK = 107;

        // COMBO 01 - Punch - Punch - Punch
        private const int D_HEIGHT_COMBO1_ATTACK01 = 139;
        private const int D_HEIGHT_COMBO1_ATTACK02 = 139;
        private const int D_HEIGHT_COMBO1_ATTACK03 = 139;
        private const int D_WIDTH_COMBO1_ATTACK01 = 186;
        private const int D_WIDTH_COMBO1_ATTACK02 = 186;
        private const int D_WIDTH_COMBO1_ATTACK03 = 186;
        private const int D_ATTACK01_COMBO1_FRAME_Y = 2;
        private const int D_ATTACK02_COMBO1_FRAME_Y = 2;
        private const int D_ATTACK03_COMBO1_FRAME_Y = 0;

        // Enemy
        private const int DRAW_WIDTH_TAKEHIT = 107;
        private const int DRAW_HEIGHT_TAKEHIT = 122;
        private const int DRAW_WIDTH_TAKEHITLOW = 107;
        private const int DRAW_HEIGHT_TAKEHITLOW = 122;

        private const int DRAW_WIDTH_GETTINGUP = 156;
        private const int DRAW_WIDTH_KNOCKDOWN = 169;

        #endregion

        DeejayCloseState state;

        // Movement 
        private float stateTime;
        private Vector2 retreatTarget;
        private int attackNumber;
        
        /// <summary>
        /// Create an enemy at a specified position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="inLevel"></param>
        public DeejayEnemy(Vector2 position, Level inLevel)
            : base(position, inLevel)
        {
            this.Health = STARTING_HEALTH;

            ResetIdleGraphic();
        }

        /// <summary>
        /// Create an enemy to spawn on a particular side of the screen
        /// </summary>
        /// <param name="startingSide"></param>
        /// <param name="inLevel"></param>
        public DeejayEnemy(DirectionTarget startingSide, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.Health = STARTING_HEALTH;
            this.FacingDir = startingSide;

            ResetIdleGraphic();
        }

        protected override void InitializeTextures()
        {
            base.InitializeTextures();

            Texture_Walk_Idle = TextureManager.GetTexure("DeejayWalkIdleSpriteSheet");
            Texture_Attack = TextureManager.GetTexure("DeejayAttackSpriteSheet");
        }

        public void SetToWait(DirectionTarget facingDir)
        {
            this.FacingDir = facingDir;

            this.state = DeejayCloseState.Wait;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_IDLE, DRAW_HEIGHT_NORMAL, 0, 0);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();
        }

        public void SetToLayingDown(DirectionTarget facingDir)
        {
            this.FacingDir = facingDir;

            this.state = DeejayCloseState.LayingDown;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_WAIT, DRAW_HEIGHT_WAIT, 0, 7);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();
        }

        public bool GetState()
        {
            return (this.state == DeejayCloseState.LayingDown);
        }

        public override void Update(GameTime gT)
        {
            switch (this.state)
            {
                #region Retreat State
                case DeejayCloseState.Retreat:

                    this.stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (this.stateTime <= 0)
                    {
                        // Ran out of time, make a decision on what to do next
                        this.DecideWhatToDo();
                    }
                    else
                    {
                        // Retreat to retreatTarget
                        // Move to X Target
                        if (this.Position.X < this.retreatTarget.X) // target is to the RIGHT
                        {
                            this.Position.X += ACTOR_X_SPEED + ACTOR_X_RETREAT_SPEED;
                            if (this.Position.X > this.retreatTarget.X) // Gone too far
                                this.Position.X = this.retreatTarget.X;
                        }
                        else // Target to the LEFT
                        {
                            this.Position.X -= ACTOR_X_SPEED + ACTOR_X_RETREAT_SPEED;
                            if (this.Position.X < this.retreatTarget.X) // Gone too far
                                this.Position.X = this.retreatTarget.X;
                        }
                        // Move to Y Location
                        if (this.Position.Y < this.retreatTarget.Y) // target is Below US
                        {
                            this.Position.Y += ACTOR_Y_SPEED + ACTOR_Y_RETREAT_SPEED;
                            if (this.Position.Y > this.retreatTarget.Y) // Gone too far
                                this.Position.Y = this.retreatTarget.Y;
                        }
                        else // Target is above us  
                        {
                            this.Position.Y -= ACTOR_Y_SPEED + ACTOR_Y_RETREAT_SPEED;
                            if (this.Position.Y < this.retreatTarget.Y) // Gone too far
                                this.Position.Y = this.retreatTarget.Y;
                        }

                        // Make sure this enemy is always facing Player
                        if (this.Position.X < this.InLevel.Player1.Position.X) // to the right of use
                            this.FacingDir = DirectionTarget.Right;
                        else // to the left
                            this.FacingDir = DirectionTarget.Left;

                        // Which animation to use
                        if (this.Position == this.retreatTarget)
                        {
                            this.InitSpriteFrames(DRAW_WIDTH_IDLE, this.DrawHeight, this.FrameX, 0);
                            this.SetOriginPosition("bottom");

                            AnimateIdle(gT);
                        }
                        else
                        {
                            this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, this.FrameX, this.FrameY);
                            this.SetOriginPosition("bottom");

                            AnimateWalking(gT);
                        }
                    }
                    break;

                #endregion

                #region MoveTo
                case DeejayCloseState.MoveTo:
                    // Are we lined up with player
                    bool linedUpX = LinedUpXWithPlayer();
                    bool linedUpY = LinedUpYWithPlayer();

                    if (linedUpX && linedUpY)
                    {
                        // Set Pre-Attack State
                        this.state = DeejayCloseState.PreAttack;
                        this.InitSpriteFrames(DRAW_WIDTH_IDLE, this.DrawHeight, 0, 0);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();
                        
                        // How long do we stay in the pre-attack state
                        this.stateTime = 0.5f * (float)Game1.Random.NextDouble();

                        break;
                    }

                    AnimateWalking(gT);
                    break;

                #endregion

                #region Pre Attack
                case DeejayCloseState.PreAttack:
                    // Am I still lined up with the player
                    if (LinedUpXWithPlayer() && LinedUpYWithPlayer())
                    {
                        // Have we been in this state long enough?
                        this.stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                        if (this.stateTime < 0)
                        {
                            // Is Player Attackable
                            if (!InLevel.Player1.IsAttackable)
                            {
                                this.GetRetreatTarget();
                                break;
                            }

                            // if (NoOtherEnemiesAttacking())
                            // {
                            // Its time to attack
                            this.state = DeejayCloseState.Attack;
                            this.Sprite = Texture_Attack;
                            this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK01, D_HEIGHT_COMBO1_ATTACK01, 0, D_ATTACK01_COMBO1_FRAME_Y);
                            if (FacingDir == DirectionTarget.Left)  this.SetOriginPosition("bottom");
                            else this.SetOriginPosition("bottom");
                            this.SetDrawArea();

                            this.attackNumber = 1;

                            SoundManager.PlaySound("ThrowPunch");
                            // this.IsAttacking = true;
                            // }
                        }
                    }
                    else
                    {
                        // Not lined up with the player
                        state = DeejayCloseState.MoveTo;
                        this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
                        this.SetOriginPosition("bottom");
                        return;
                    }

                    AnimateIdle(gT);
                    break;
                #endregion

                #region Attacks

                case DeejayCloseState.Attack:
                    // Do Nothing
                    switch (attackNumber)
                    {
                        case 1: // Animate 
                            AnimateAttack1(gT);
                            break;
                        case 2: // Animate 
                            AnimateAttack2(gT);
                            break;
                        case 3: // Animate 
                            AnimateAttack3(gT);
                            break;

                    }
                    break;
                #endregion

                #region Take Hit and Die Cycle
                case DeejayCloseState.TakeHit:
                    this.SetOriginPosition("bottom");
                    AnimateTakeHit(gT);
                    break;

                case DeejayCloseState.TakeHitLowKick:
                    this.SetOriginPosition("bottom");
                    AnimateTakeHitKick(gT);
                    break;

                case DeejayCloseState.KnockedDown:
                    this.SetOriginPosition("bottom");
                    AnimateKnockDown(gT);
                    break;

                case DeejayCloseState.Down:
                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime >= Actor.DOWN_TIME)
                    {
                        // Set up Gettign Up Animation
                        this.state = DeejayCloseState.GettingUp;
                        this.CurrentFrameTime = 0;
                        this.InitSpriteFrames(DRAW_WIDTH_GETTINGUP, DRAW_HEIGHT_NORMAL, 0, 6);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();
                    }
                    break;

                case DeejayCloseState.GettingUp:
                    AnimateGettingUp(gT);
                    break;

                case DeejayCloseState.Dying:
                    // Flash the Body a few times
                    this.stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (this.stateTime <= 0)
                    {
                        this.stateTime = ENEMY_DEATH_FLASH_TIME;
                        this.IsVisible = !this.IsVisible;
                        this.DeathFlashes++;

                        if (this.DeathFlashes >= 8)
                        {
                            // Will I drop a item
                            if (Game1.Random.NextDouble() >= 0.5f) // 0.9 = 10% chance of dropping item
                            {
                                this.InLevel.GameItems.Add(new PickUpStone(this.InLevel, this.Position));
                            }

                            // Actor is Dead
                            this.RemoveActorFromLevel();
                        }
                    }
                    break;

                #endregion

                #region Waiting

                case DeejayCloseState.Wait:

                    AnimateWaiting(gT);

                    break;

                #endregion

                #region Laying Down

                case DeejayCloseState.LayingDown:

                    AnimateLayingDownWaiting(gT);

                    break;

                #endregion

            }
        }

        public override void Draw(SpriteBatch SB)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);
            SpriteEffects sprEffect = SpriteEffects.None;

            this.GetLayerDepth(this.Position.Y);

            if (this.IsVisible)
            {
                // Facing Left or Right?
                if (FacingDir == DirectionTarget.Right) sprEffect = SpriteEffects.None;
                else sprEffect = SpriteEffects.FlipHorizontally;

                SB.Draw(this.Sprite, drawPos, this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, sprEffect, LayerDepth);
            }

            #region HealthBar
            // Red Health Bar
            SB.Draw(Texture_SinglePixel, new Vector2(drawPos.X - HEALTH_BAR_WIDTH / 2, drawPos.Y - DRAW_HEIGHT_NORMAL - HEALTH_BAR_HEIGHT),
                new Rectangle(0, 0, HEALTH_BAR_WIDTH + 2, HEALTH_BAR_HEIGHT + 2), new Color(Color.Red, 0.4f), 0f,
                Vector2.Zero, 1f, SpriteEffects.None, this.LayerDepth + 0.001f);

            // How long do we draw the Enemy's Health Bar
            float percent = this.Health / STARTING_HEALTH;
            int drawWidth = (int)(percent * HEALTH_BAR_WIDTH);

            // Yellow Health Bar
            SB.Draw(Texture_SinglePixel, new Vector2(drawPos.X - HEALTH_BAR_WIDTH / 2 + 1, drawPos.Y - DRAW_HEIGHT_NORMAL - HEALTH_BAR_HEIGHT + 1),
            new Rectangle(0, 0, drawWidth, HEALTH_BAR_HEIGHT), new Color(Color.Yellow, 0.4f), 0f,
            Vector2.Zero, 1f, SpriteEffects.None, this.LayerDepth);

            #endregion

            base.Draw(SB);
        }

        public override void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);

            // Facing Left or Right?
            if (FacingDir == DirectionTarget.Right)
                SB.Draw(this.Sprite, drawPos, this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.None, layerDepth);
            else // We must be facing to the left
                SB.Draw(this.Sprite, drawPos, this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.FlipHorizontally, layerDepth);
        }

        #region AI Methods

        /// <summary>
        /// Resets player to it's idle state, awaiting player input
        /// </summary>
        public void ResetIdleGraphic()
        {
            this.IsAttacking = false;
            this.IsAttackable = true;

            this.Sprite = Texture_Walk_Idle;
            this.CurrentFrameTime = 0;
            this.InitSpriteFrames(DRAW_WIDTH_IDLE, DRAW_HEIGHT_NORMAL, 0, 0);
            this.SetDrawArea();
            this.SetOriginPosition("bottom");
            this.UpdateHitArea();

            if (InLevel.Player1 != null) this.DecideWhatToDo();
        }

        public void GetUpAnimation()
        {
            this.IsAttacking = false;
            this.IsAttackable = true;
            
            this.state = DeejayCloseState.GettingUp;
            this.CurrentFrameTime = 0;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_GETTINGUP, DRAW_HEIGHT_TAKEHIT, 0, 6);
            this.SetDrawArea();
            this.SetOriginPosition("bottom");
            this.UpdateHitArea();
        }

        private void DecideWhatToDo()
        {
            if (Game1.Random.NextDouble() < 0.5d)
            {
                // Decide to retreat
                GetRetreatTarget();

                // Set time to be in Retreat STate
                stateTime = (float)(Game1.Random.NextDouble() + 1.8);
            }
            else
            {
                // ATTACK
                if (NoOtherEnemiesAttacking())
                {
                    this.IsAttacking = true;
                    this.state = DeejayCloseState.MoveTo;
                    this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
                }
                //else // ???????????????????????? - They wont wait until other enemy is done fighting with player
                //{
                //   GetRetreatTarget();
                //}
            }
        }
        private void GetRetreatTarget()
        {
            state = DeejayCloseState.Retreat;

            // Retreat to which side of the player
            if (Game1.Random.NextDouble() < 0.5d)
            {
                // Go LEFT of the player
                retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X - 200),
                    (int)(InLevel.Player1.Position.X - 100));

                // Is this position off screen
                if (retreatTarget.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                {
                    // go to the Right Side of Player
                    retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X + 100),
                        (int)(InLevel.Player1.Position.X + 200));
                }
            }
            else
            {
                // go to the Right Side of Player
                retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X + 100),
                    (int)(InLevel.Player1.Position.X + 200));

                // Is this position off screen
                if (retreatTarget.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                {
                    // Go LEFT of the player
                    retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X - 200),
                        (int)(InLevel.Player1.Position.X - 100));

                }

            }

            // Get Y Retreat Target
            retreatTarget.Y = Game1.Random.Next(InLevel.PlayBounds.Top, InLevel.PlayBounds.Bottom);
        }

        private bool LinedUpXWithPlayer()
        {
            // is the player to the left
            if (InLevel.Player1.Position.X <= this.Position.X - 80) // Player is to the left
            {
                // Move left
                this.Position.X -= ACTOR_X_SPEED;
                FacingDir = DirectionTarget.Left;
                return false;
            }
            else if (InLevel.Player1.Position.X >= this.Position.X + 80) // Player is to the right
            {
                // Move Right
                this.Position.X += ACTOR_X_SPEED;
                FacingDir = DirectionTarget.Right;
                return false;
            }
            else
            {
                return true;
            }


        }
        private bool LinedUpYWithPlayer()
        {
            // Is the player above or below
            if (InLevel.Player1.Position.Y <= this.Position.Y - 8) // Player is ABOVE
            {
                // Move enemy UP a bit
                this.Position.Y -= ACTOR_Y_SPEED;
                return false;
            }
            else if (InLevel.Player1.Position.Y >= this.Position.Y + 8) // Player is BELOW
            {
                // Move enemy down a bit
                this.Position.Y += ACTOR_Y_SPEED;
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool NoOtherEnemiesAttacking()
        {
            // Loop through all actors
            // See if anybody else is attacking
            for (int i = 0; i < InLevel.Actors.Count; i++)
            {
                if (InLevel.Actors[i].IsAttacking)
                {
                    return false; // Someone else is attacking
                }
            }
            // If we reach here no one else is attacking
            return true;
        }

        #endregion

        #region Collision Detections Methods
        
        public override void GetHit(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;
            this.Health -= damage;

            if (this.CheckForDeath())
            {
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            this.state = DeejayCloseState.TakeHit;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHIT, DRAW_HEIGHT_TAKEHIT, 0, 2);
            // this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left) FacingDir = DirectionTarget.Right;
            else FacingDir = DirectionTarget.Left;

            SoundManager.PlaySound("GetHit01");
        }

        public override void GetHitKick(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;
            this.Health -= damage;

            if (this.CheckForDeath())
            {
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            this.state = DeejayCloseState.TakeHitLowKick;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHITLOW, DRAW_HEIGHT_TAKEHITLOW, 0, 3);
            // this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left) FacingDir = DirectionTarget.Right;
            else FacingDir = DirectionTarget.Left;

            SoundManager.PlaySound("GetHit02");
        }

        public override void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
            // Set state and texture
            this.IsAttackable = false;
            this.IsAttacking = false;
            this.Health -= damage;

            this.state = DeejayCloseState.KnockedDown;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_KNOCKDOWN, DRAW_HEIGHT_TAKEHIT, 0, 4);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Right) FacingDir = DirectionTarget.Left;
            else FacingDir = DirectionTarget.Right;

            SoundManager.PlaySound("GetHit03");
        }

        private void CheckPlayerCollision()
        {
            Actor actor;

            UpdateHitArea();

            for (int i = InLevel.Actors.Count - 1; i >= 0; i--)
            {
                // Make sure our not looking at ourself
                actor = InLevel.Actors[i] as EnemyClose;
                if (actor == this)
                    continue;

                // Are we looking at the Player?
                actor = InLevel.Actors[i] as Player;
                if (actor != null)
                {
                    // Update the current actors Hit Area
                    actor.UpdateHitArea();

                    // 1) Is Actor/Enemy attackable?
                    if (actor.IsAttackable)
                    {
                        // 2) Are wer within Y Range
                        if (actor.Position.Y > this.Position.Y - HIT_Y_RANGE
                            && actor.Position.Y < this.Position.Y + HIT_Y_RANGE)
                        {
                            // 3) Which way is the enemy facing
                            if (this.FacingDir == DirectionTarget.Left)
                            {
                                // 4) Is the enemy/actor in front of us **LEFT**
                                if (actor.Position.X < this.Position.X)
                                {
                                    // 5) Players left edge <MORE LEFT> than actors RIGHT
                                    if (this.Position.X - HitArea < actor.Position.X + actor.HitArea)
                                    {
                                        HitSomeone(actor);
                                    }
                                }
                            }
                            //  3) Which way is the player facing
                            else
                            {
                                // A) is Actor in in front of us or RIGHT OF the player
                                if (actor.Position.X > this.Position.X)
                                {
                                    // 5) Players RIGHT EDGE is more right than actor's LEFT 
                                    if (this.Position.X + HitArea > actor.Position.X - actor.HitArea)
                                    {
                                        HitSomeone(actor);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetEffect(string effect, Actor who, DeejayEnemy p)
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, who.Position, InLevel, p);
            InLevel.GameItems.Add(spark);
        }

        private void HitSomeone(Actor whoToHit)
        {
            switch (attackNumber)
            {
                case 1: // Straight Jab
                case 2: // Upper Cut
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetHit(FacingDir, 10);
                    break;

                case 3: // Side Kick
                    SetEffect("smallspark", whoToHit, this);
                    whoToHit.GetKnockedDown(FacingDir, 20);
                    break;
            }
        }

        #endregion

        #region Animations

        private void AnimateIdle(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 6) this.FrameX = 0;

                this.SetDrawArea();
            }
        }

        private void AnimateTakeHit(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 3)
                {
                    ResetIdleGraphic();
                    return;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateTakeHitKick(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 3)
                {
                    ResetIdleGraphic();
                    return;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateKnockDown(GameTime gT)
        {
            float calcDistance = 0;
        
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 0.5f)
            {
                if (this.FacingDir == DirectionTarget.Left) 
                    calcDistance = ((this.Position.X + 150) - this.Position.X) * 20f * Camera.Elasticity;
                else
                    calcDistance = ((this.Position.X - 150) - this.Position.X) * 20f * Camera.Elasticity;

                this.Position.X = this.Position.X + calcDistance;

                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX == 1 && CheckForDeath() && this.FrameY == 4)
                {
                    SoundManager.PlaySound("GetHit-Died");
                }

                if (this.FrameX > 3 && this.FrameY == 5)
                {
                    if (CheckForDeath())
                    {
                        state = DeejayCloseState.Dying;
                        stateTime = 1f;
                        this.FrameX = 2;
                        return;
                    }

                    // Set state and texture
                    state = DeejayCloseState.Down;
                    this.FrameX = 2;
                    stateTime = 0f;
                    return;
                }

                if (this.FrameX > 6 && this.FrameY == 4)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateGettingUp(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            float framePerSec = 1.5f;

            if (this.CurrentFrameTime >= Actor.FrameRate * framePerSec)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 6)
                {
                    ResetIdleGraphic();
                    return;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateWalking(GameTime gT)
        {
            this.SetOriginPosition("bottom");

            // Just in case we are coming from an animation which doesn't have this.FrameY
            // set to 1, or 2
            // if (this.FrameY != 1 && this.FrameY != 2)
            this.FrameY = 1;

            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.5f; // * float makes it faster or slower
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0;
                this.FrameX++;

                if (this.FrameY == 1)
                {
                    if (this.FrameX > 4) this.FrameX = 0;
                }
                else // FrameY must be equal to 2
                {
                    if (this.FrameX > 4) this.FrameX = 0;
                }

                this.SetDrawArea();
            }
        }

        // Combo 1 Animations ( Punch - Upper Punch - Side Kick )
        private void AnimateAttack1(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds; // Speed

            if (this.CurrentFrameTime >= Actor.FrameRate * 0.5f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                // Collision Detection
                if (this.FrameX == 1 || this.FrameX == 4) this.CheckPlayerCollision();

                if (this.FrameX > 4)
                {
                    // Setup Attack 02 
                    this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK02, this.DrawHeight, 0, 3);

                    if (FacingDir == DirectionTarget.Left) this.SetOriginPosition("bottom", 18, 2);
                    else this.SetOriginPosition("bottom", -18, 2);
                   
                    this.attackNumber++;

                    SoundManager.PlaySound("ThrowPunch");

                    return;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateAttack2(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 0.8f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                // Collision Detection
                if (this.FrameX == 3 && this.FrameY == 3) this.CheckPlayerCollision();

                if (this.FrameX > 1 && this.FrameY == 4)
                {
                    // Setup Attack 02 
                    this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK03, this.DrawHeight, 0, 0);

                    if (FacingDir == DirectionTarget.Left) this.SetOriginPosition("bottom", 18, 2);
                    else this.SetOriginPosition("bottom", -18, 2);

                    this.attackNumber++;

                    SoundManager.PlaySound("ThrowPunch");
                    return;
                }

                if (this.FrameX > 5 && this.FrameY == 3)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateAttack3(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 0.5f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 5)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (this.FrameX == 3) this.CheckPlayerCollision();

                this.SetDrawArea();
            }
        }

        private void AnimateWaiting(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds * 0.8f;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX > 6) this.FrameX = 0;

                this.SetDrawArea();
            }
        }

        private void AnimateLayingDownWaiting(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds * 0.4f;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX > 3) this.FrameX = 0;

                this.SetDrawArea();
            }
        }

        #endregion
    }
}
