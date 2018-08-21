using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects.Items;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Enemies
{
    enum AdonCloseState
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
        Level01Intro
    }

    class AdonEnemy : ActorEnemy
    {
        #region constants

        const int HEALTH_BAR_WIDTH = 60;
        const int HEALTH_BAR_HEIGHT = 15;
        const float STARTING_HEALTH = 450;
        const float ACTOR_X_SPEED = 2f;
        const float ACTOR_Y_SPEED = 2f;
        const float ACTOR_X_RETREAT_SPEED = 0f;
        const float ACTOR_Y_RETREAT_SPEED = 0f;

        #endregion

        #region draw area constants

        // WAIT - Not doing anything, show this one
        private const int DRAW_HEIGHT_WAIT = 122;
        private const int DRAW_WIDTH_WAIT = 175;
        private const int DRAW_HEIGHT_WIN = 122;
        private const int DRAW_WIDTH_WIN = 175;

        private const int DRAW_HEIGHT_NORMAL = 122;
        private const int DRAW_WIDTH_IDLE = 90;
        private const int DRAW_WIDTH_WALK = 115;

        // COMBO 01 - Punch - Punch - Punch
        private const int D_HEIGHT_COMBO1_ATTACK01 = 130;
        private const int D_HEIGHT_COMBO1_ATTACK02 = 130;
        private const int D_HEIGHT_COMBO1_ATTACK03 = 130;
        private const int D_WIDTH_COMBO1_ATTACK01 = 135;
        private const int D_WIDTH_COMBO1_ATTACK02 = 135;
        private const int D_WIDTH_COMBO1_ATTACK03 = 135;
        private const int D_ATTACK01_COMBO1_FRAME_Y = 0;
        private const int D_ATTACK02_COMBO1_FRAME_Y = 0;
        private const int D_ATTACK03_COMBO1_FRAME_Y = 1;

        // Enemy
        private const int DRAW_WIDTH_TAKEHIT = 140;
        private const int DRAW_HEIGHT_TAKEHIT = 122;
        private const int DRAW_WIDTH_TAKEHITLOW = 169;
        private const int DRAW_HEIGHT_TAKEHITLOW = 122;

        private const int DRAW_WIDTH_GETTINGUP = 156;
        private const int DRAW_WIDTH_KNOCKDOWN = 169;

        #endregion

        AdonCloseState state;

        // Texture Details
        // Texture2D texture;
        // Vector2 originCharacters;
        //Rectangle drawArea;
        //float currentFrameTime;
        //int drawWidth;
        //int drawHeight;
        //int frameX;
        //int frameY;
        // Color drawColor;

        // Movement 
        float stateTime;
        Vector2 retreatTarget;
        int attackNumber;

        // Cutscene
        Vector2 targetPosition;

        /// <summary>
        /// Create an enemy at a specified position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="inLevel"></param>
        public AdonEnemy(Vector2 position, Level inLevel)
            : base(position, inLevel)
        {
            this.ResetIdleGraphic();
            
            this.Health = STARTING_HEALTH;
        }

        protected override void InitializeTextures()
        {
            base.InitializeTextures();

            Texture_Walk_Idle = TextureManager.GetTexure("AdonWalkIdleSpriteSheet");
            Texture_Attack = TextureManager.GetTexure("AdonAttackSpriteSheet");
        }

        /// <summary>
        /// Create an enemy to spawn on a particular side of the screen
        /// </summary>
        /// <param name="startingSide"></param>
        /// <param name="inLevel"></param>
        public AdonEnemy(DirectionTarget startingSide, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.FacingDir = startingSide;

            ResetIdleGraphic();

            this.Health = STARTING_HEALTH;
        }

        public void SetToWait(DirectionTarget facingDir)
        {
            this.Sprite = Texture_Walk_Idle;

            this.InitSpriteFrames(DRAW_WIDTH_WAIT, DRAW_HEIGHT_WAIT, 0, 7);
            this.SetDrawArea();
            this.SetOriginPosition("bottom");

            this.state = AdonCloseState.Wait;
            this.FacingDir = facingDir;
        }

        public void SetIntro01TargetPosition(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
            this.state = AdonCloseState.Level01Intro;
            
            // Setup Walk Animation
            this.InitSpriteFrames(DRAW_WIDTH_WALK, DRAW_HEIGHT_NORMAL);
            this.Sprite = Texture_Walk_Idle;

            this.SetDrawArea();
        }

        public override void Update(GameTime gT)
        {
            switch (state)
            {
                #region Retreat State
                case AdonCloseState.Retreat:

                    stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime <= 0)
                    {
                        // Run out of time
                        DecideWhatToDo();
                    }
                    else
                    {
                        // Retreat to retreatTarget
                        // Move to X Target
                        if (Position.X < retreatTarget.X) // target is to the RIGHT
                        {
                            this.Position.X += ACTOR_X_SPEED + ACTOR_X_RETREAT_SPEED;
                            if (Position.X > retreatTarget.X) // Gone too far
                                Position.X = retreatTarget.X;
                        }
                        else // Target to the LEFT
                        {
                            this.Position.X -= ACTOR_X_SPEED + ACTOR_X_RETREAT_SPEED;
                            if (Position.X < retreatTarget.X) // Gone too far
                                Position.X = retreatTarget.X;
                        }
                        // Move to Y Location
                        if (Position.Y < retreatTarget.Y) // target is Below US
                        {
                            this.Position.Y += ACTOR_Y_SPEED + ACTOR_Y_RETREAT_SPEED;
                            if (Position.Y > retreatTarget.Y) // Gone too far
                                Position.Y = retreatTarget.Y;
                        }
                        else // Target is above us  
                        {
                            this.Position.Y -= ACTOR_Y_SPEED + ACTOR_Y_RETREAT_SPEED;
                            if (Position.Y < retreatTarget.Y) // Gone too far
                                Position.Y = retreatTarget.Y;
                        }

                        // Make sure this enemy is always facing Player
                        if (Position.X < InLevel.Player1.Position.X) // to the right of use
                            this.FacingDir = DirectionTarget.Right;
                        else // to the left
                            this.FacingDir = DirectionTarget.Left;

                        // Which animation to use
                        if (this.Position == retreatTarget)
                        {
                            // At location IDLE
                            this.FrameY = 0;
                            this.DrawWidth = DRAW_WIDTH_IDLE;
                            //drawHeight = DRAW_HEIGHT_NORMAL; ;
                            this.SetOriginPosition("bottom");
                            this.AnimateIdle(gT);

                        }
                        else
                        {
                            // Not at location
                            this.DrawWidth = DRAW_WIDTH_WALK;
                            //drawHeight = DRAW_HEIGHT_NORMAL;
                            this.SetOriginPosition("bottom");
                            this.AnimateWalking(gT);
                        }
                    }
                    break;

                #endregion

                #region MoveTo
                case AdonCloseState.MoveTo:
                    // Are we lined up with player
                    bool linedUpX = LinedUpXWithPlayer();
                    bool linedUpY = LinedUpYWithPlayer();

                    if (linedUpX && linedUpY)
                    {
                        // Set Pre-Attack State
                        this.InitSpriteFrames(DRAW_WIDTH_IDLE, this.DrawHeight, 0, 0);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();

                        state = AdonCloseState.PreAttack;
                       
                        // How long do we stay in the pre-attack state
                        stateTime = 0.5f * (float)Game1.Random.NextDouble();
                        break;
                    }

                    AnimateWalking(gT);
                    break;

                #endregion

                #region Pre Attack
                case AdonCloseState.PreAttack:
                    // Am I still lined up with the player
                    if (LinedUpXWithPlayer() && LinedUpYWithPlayer())
                    {
                        // Have we been in this state long enough?
                        stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                        if (stateTime < 0)
                        {
                            // Is Player Attackable
                            if (!InLevel.Player1.IsAttackable)
                            {
                                GetRetreatTarget();
                                break;
                            }

                            // if (NoOtherEnemiesAttacking())
                            //{

                            // Its time to attack
                            this.Sprite = Texture_Attack;
                            this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK01, D_HEIGHT_COMBO1_ATTACK01, 0, D_ATTACK01_COMBO1_FRAME_Y);
                            this.SetOriginPosition("bottom");
                            this.SetDrawArea();

                            this.state = AdonCloseState.Attack;
                            this.attackNumber = 1;

                            SoundManager.PlaySound("ThrowPunch");
                            //this.IsAttacking = true;
                            //}
                        }
                    }
                    else
                    {
                        // Not lined up with the player
                        this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
                        this.SetOriginPosition("bottom");
                       
                        this.state = AdonCloseState.MoveTo;
                       
                        return;
                    }

                    AnimateIdle(gT);
                    break;
                #endregion

                #region Attacks

                case AdonCloseState.Attack:
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
                case AdonCloseState.TakeHit:
                    this.SetOriginPosition("bottom");
                    AnimateTakeHit(gT);
                    break;

                case AdonCloseState.TakeHitLowKick:
                    this.SetOriginPosition("bottom");
                    AnimateTakeHitKick(gT);
                    break;

                case AdonCloseState.KnockedDown:
                    this.SetOriginPosition("bottom");
                    AnimateKnockDown(gT);
                    break;

                case AdonCloseState.Down:
                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime >= Actor.DOWN_TIME)
                    {
                        this.CurrentFrameTime = 0f;
                        this.InitSpriteFrames(DRAW_WIDTH_GETTINGUP, DRAW_HEIGHT_NORMAL, 0, 6);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();

                        // Set up Gettign Up Animation
                        state = AdonCloseState.GettingUp;
                    }
                    break;

                case AdonCloseState.GettingUp:
                    AnimateGettingUp(gT);
                    break;

                case AdonCloseState.Dying:
                    // Flash the Body a few times
                    stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime <= 0)
                    {
                        stateTime = ENEMY_DEATH_FLASH_TIME;
                        IsVisible = !IsVisible;
                        DeathFlashes++;

                        if (DeathFlashes >= 8)
                        {
                            // Will I drop a item
                            if (Game1.Random.NextDouble() >= 0.9f) // 0.9 = 10% chance of dropping item
                            {
                                this.InLevel.GameItems.Add(
                                    new PickUpStone(this.InLevel, this.Position));
                            }
                            // Actor is Dead
                            RemoveActorFromLevel();
                        }
                    }
                    break;

                #endregion

                #region Waiting

                case AdonCloseState.Wait:

                    AnimateWaiting(gT);

                    break;

                #endregion

                #region CutScene Walking out
                case AdonCloseState.Level01Intro:
                    if (this.Position != targetPosition)
                    {
                        // Move to the destination
                        LineUpXTargetPos();
                        LineUpYTargetPos();
                        AnimateWalking(gT);

                    } 
                    else
                    {
                        RemoveActorFromLevel();

                    }
                    break;
                #endregion


            }
        }

        public override void Draw(SpriteBatch SB)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);
            SpriteEffects sprEffect = SpriteEffects.None;

            this.GetLayerDepth(Position.Y);

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
            int drawHealthBarWidth = (int)(percent * HEALTH_BAR_WIDTH);

            // Yellow Health Bar
            SB.Draw(Texture_SinglePixel, new Vector2(drawPos.X - HEALTH_BAR_WIDTH / 2 + 1, drawPos.Y - DRAW_HEIGHT_NORMAL - HEALTH_BAR_HEIGHT + 1),
            new Rectangle(0, 0, drawHealthBarWidth, HEALTH_BAR_HEIGHT), new Color(Color.Yellow, 0.4f), 0f,
            Vector2.Zero, 1f, SpriteEffects.None, this.LayerDepth);

            #endregion

            base.Draw(SB);
        }

        public override void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);

            // Facing Left or Right?
            if (FacingDir == DirectionTarget.Right)
                SB.Draw(this.Sprite, drawPos, this.DrawArea,
                    this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.None, layerDepth);
            else // We must be facing to the left
                SB.Draw(this.Sprite, drawPos, this.DrawArea,
                    this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.FlipHorizontally, layerDepth);

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
            this.SetOriginPosition("bottom");
            this.SetDrawArea();
            this.HitArea = this.DrawWidth / 2;

            if (InLevel.Player1 != null) DecideWhatToDo();
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
                    this.state = AdonCloseState.MoveTo;
                 
                    this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
                    this.SetOriginPosition("bottom");
                    this.SetDrawArea();
                }
                //else // ???????????????????????? - They wont wait until other enemy is done fighting with player
                //{
                //   GetRetreatTarget();
                //}
            }
        }
        private void GetRetreatTarget()
        {
            state = AdonCloseState.Retreat;

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
            //int range = InLevel.PlayBounds.Bottom - InLevel.PlayBounds.Top;
            //float percent = (float)Game1.Random.NextDouble();
            //retreatTarget.Y = percent * range + InLevel.PlayBounds.Top;
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

        private void LineUpXTargetPos()
        {
            // is the targetPosition to the left
            if (targetPosition.X < this.Position.X) // TargetPosition is to the left
            {
                // Move left
                this.Position.X -= ACTOR_X_SPEED;
                FacingDir = DirectionTarget.Left;
                if (targetPosition.X >= this.Position.X)
                {
                    this.Position.X = targetPosition.X;
                }

            }
            else if (targetPosition.X > this.Position.X) // TargetPosition is to the right
            {
                // Move Right
                this.Position.X += ACTOR_X_SPEED;
                FacingDir = DirectionTarget.Right;
                if (targetPosition.X <= this.Position.X)
                {
                    this.Position.X = targetPosition.X;
                }

            }


        }

        private void LineUpYTargetPos()
        {
            // is the targetPosition above or below
            if (targetPosition.Y < this.Position.Y) // TargetPosition is above
            {
                // Move Up
                this.Position.Y -= ACTOR_Y_SPEED;
                if (targetPosition.Y >= this.Position.Y)
                {
                    this.Position.Y = targetPosition.Y;
                }
            }
            else if (targetPosition.Y > this.Position.Y) // TargetPosition is below
            {
                // Move Down
                this.Position.Y += ACTOR_Y_SPEED;
                if (targetPosition.Y <= this.Position.Y)
                {
                    this.Position.Y = targetPosition.Y;
                }
            }


        }

        #endregion

        #region Collision Detections Methods
       
        public override void GetHit(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;
            this.Health -= damage;

            if (CheckForDeath())
            {
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            state = AdonCloseState.TakeHit;

            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHIT, DRAW_HEIGHT_TAKEHIT, 0, 2);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit01");

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left)
                FacingDir = DirectionTarget.Right;
            else
                FacingDir = DirectionTarget.Left;
        }

        public override void GetHitKick(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;

            this.Health -= damage;
            if (CheckForDeath())
            {
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            this.state = AdonCloseState.TakeHitLowKick;

            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHITLOW, DRAW_HEIGHT_TAKEHITLOW, 0, 4);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit02");

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left)
                FacingDir = DirectionTarget.Right;
            else
                FacingDir = DirectionTarget.Left;

        }

        public override void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
            // Set state and texture
            this.IsAttackable = false;
            this.IsAttacking = false;
            this.state = AdonCloseState.KnockedDown;

            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_KNOCKDOWN, DRAW_HEIGHT_TAKEHIT, 0, 4);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit03");

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Right)
                FacingDir = DirectionTarget.Left;
            else
                FacingDir = DirectionTarget.Right;

            this.Health -= damage;
        }

        private void CheckPlayerCollision()
        {
            UpdateHitArea();

            for (int i = InLevel.Actors.Count - 1; i >= 0; i--)
            {
                Actor actor;

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

        private void SetEffect(string effect, Actor who, AdonEnemy p)
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
                    whoToHit.GetHit(FacingDir, 20);
                    break;

                case 3: // Side Kick
                    SetEffect("bluespark", whoToHit, this);  
                    whoToHit.GetKnockedDown(FacingDir, 40);
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
                if (this.FrameX > 3) this.FrameX = 0;

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
                    this.ResetIdleGraphic();
                    return;
                }

                SetDrawArea();
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
                    this.ResetIdleGraphic();
                    return;
                }

                SetDrawArea();
            }
        }

        private void AnimateKnockDown(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 1.2f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX == 1 && CheckForDeath() && this.FrameY == 4)
                {
                    SoundManager.PlaySound("GetHit-Died");
                }
                if (this.FrameX > 4 && this.FrameY == 5)
                {
                    if (CheckForDeath())
                    {
                        state = AdonCloseState.Dying;
                        stateTime = 1f;
                        this.FrameX = 4;
                        return;
                    }
                    // Set state and texture
                    state = AdonCloseState.Down;
                    this.FrameX = 4;

                    stateTime = 0f;
                    return;
                }
                if (this.FrameX > 4 && this.FrameY == 4)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                SetDrawArea();
            }
        }
        private void AnimateGettingUp(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            float framePerSec = 1f;

            if (this.CurrentFrameTime >= Actor.FrameRate * framePerSec)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX > 6)
                {
                    ResetIdleGraphic();
                    return;
                }

                SetDrawArea();
            }
        }

        private void AnimateWalking(GameTime gT)
        {
            this.SetOriginPosition("bottom");
           
            // Just in case we are coming from an animation which doesn't have frameY
            // set to 1, or 2
            // if (frameY != 1 && frameY != 2)
            this.FrameY = 1;

            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.5f; // * float makes it faster or slower
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0;
                this.FrameX++;

                if (this.FrameY == 1)
                {
                    if (this.FrameX > 5)
                    {
                        this.FrameX = 0;
                    }
                }
                else // FrameY must be equal to 2
                {
                    if (this.FrameX > 5)
                    {
                        this.FrameX = 0;
                    }
                }

                SetDrawArea();
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
                if (this.FrameX == 2) CheckPlayerCollision();

                if (this.FrameX > 4)
                {
                    // Setup Attack 02 
                    this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK02, this.DrawHeight, 5, 0);
                    if (FacingDir == DirectionTarget.Left) this.SetOriginPosition("bottom");
                    else this.SetOriginPosition("bottom");
                    this.SetDrawArea();
                    attackNumber++;
                    SoundManager.PlaySound("ThrowPunch");
                    return;
                }

                SetDrawArea();
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
                if (this.FrameX == 7 && this.FrameY == 0) CheckPlayerCollision();

                if (this.FrameX > 7 && this.FrameY == 0)
                {
                    // Setup Attack 02 
                    this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK03, this.DrawHeight, 0, 1);
                    if (FacingDir == DirectionTarget.Left) this.SetOriginPosition("bottom");
                    else this.SetOriginPosition("bottom");
                    this.SetDrawArea();
                    this.attackNumber++;
                    SoundManager.PlaySound("ThrowPunch");
                    return;
                }

                SetDrawArea();
            }
        }

        private void AnimateAttack3(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 0.5f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX > 7)
                {
                    ResetIdleGraphic();
                    return;
                }
                // Collision Detection
                if (this.FrameX == 4) CheckPlayerCollision();

                SetDrawArea();
            }
        }

        private void AnimateWaiting(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds * 0.4f;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;
                if (this.FrameX > 5) this.FrameX = 0;

                SetDrawArea();
            }
        }

        #endregion

    }
}
