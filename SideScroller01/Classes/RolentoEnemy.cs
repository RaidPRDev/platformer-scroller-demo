using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{

    enum RolentoEnemyRangedState
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
        MoveAway,
        ThrowStone
    }

    class RolentoEnemy : Actor
    {
        #region constants

        const int HEALTH_BAR_WIDTH = 60;
        const int HEALTH_BAR_HEIGHT = 15;
        const float STARTING_HEALTH = 150;
        const int THROW_STONE_SAFE_DISTANCE = 300;
        const float ACTOR_X_SPEED = 2f;
        const float ACTOR_Y_SPEED = 2f;
        const float ACTOR_X_RETREAT_SPEED = 0f;
        const float ACTOR_Y_RETREAT_SPEED = 0f;

        #endregion

        #region draw area constants

        // WAIT - Not doing anything, show this one
        private const int DRAW_HEIGHT_NORMAL = 122;
        private const int DRAW_WIDTH_IDLE = 107;
        private const int DRAW_WIDTH_WALK = 107;
 
        // COMBO 01 - Punch - Punch - Punch
        private const int D_HEIGHT_COMBO1_ATTACK01 = 139;
        private const int D_WIDTH_COMBO1_ATTACK01 = 186;

        // Enemy
        private const int DRAW_WIDTH_TAKEHIT = 107;
        private const int DRAW_HEIGHT_TAKEHIT = 122;
        private const int DRAW_WIDTH_TAKEHITLOW = 156;
        private const int DRAW_HEIGHT_TAKEHITLOW = 122;
        private const int DRAW_WIDTH_GETTINGUP = 156;
        private const int DRAW_HEIGHT_GETTINGUP = 122;
        private const int DRAW_WIDTH_KNOCKDOWN = 156;

        // Game Items
        private const int DRAW_WIDTH_THROW = 186;
        private const int DRAW_HEIGHT_THROW = 139;

        #endregion

        RolentoEnemyRangedState state;

        // Texture Details

        Texture2D texture;
        Vector2 originCharacters;
        Rectangle drawArea;
        float currentFrameTime;
        int drawWidth;
        int drawHeight;
        int frameX;
        int frameY;
        Color drawColor;

        // Movement 
        float stateTime;
        Vector2 retreatTarget;

        public RolentoEnemy(Vector2 position, Level inLevel)
            : base(position, inLevel)
        {
            
            ResetIdleGraphic();
            drawColor = Color.White;
                //new Color(0.2f, 0.2f, 1f); // Set the draw color
            this.Health = STARTING_HEALTH;

        }

            /// <summary>
        /// Create an enemy to spawn on a particular side of the screen
        /// </summary>
        /// <param name="startingSide"></param>
        /// <param name="inLevel"></param>
        public RolentoEnemy(DirectionTarget startingSide, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.FacingDir = startingSide;

            ResetIdleGraphic();
            drawColor = Color.White;
            this.Health = STARTING_HEALTH;

        }

        public RolentoEnemy(DirectionTarget startingSide, Vector2 position, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.FacingDir = startingSide;
            
            ResetIdleGraphic();
            drawColor = Color.White;
            this.Health = STARTING_HEALTH;

        }


        public override void Update(GameTime gT)
        {
            switch (state)
            {
                #region Retreat State
                case RolentoEnemyRangedState.Retreat:

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
                            frameY = 0;
                            drawWidth = DRAW_WIDTH_IDLE;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            AnimateIdle(gT);

                        }
                        else
                        {
                            // Not at location
                            frameY = 2;
                            drawWidth = DRAW_WIDTH_WALK;
                            drawHeight = DRAW_HEIGHT_NORMAL;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            AnimateWalking(gT);

                        }
                    }
                    break;

                #endregion

                #region MoveTo - move to the Player, for attacking
                case RolentoEnemyRangedState.MoveTo:
                    // Are we lined up with player
                    bool linedUpX = LinedUpXWithPlayerClose();
                    bool linedUpY = LinedUpYWithPlayer();

                    if (linedUpX && linedUpY)
                    {
                        // Set Pre-Attack State
                        frameX = 0;
                        frameY = 0;
                        state = RolentoEnemyRangedState.PreAttack;
                        drawWidth = DRAW_WIDTH_IDLE;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        SetDrawArea();

                        // How long do we stay in the pre-attack state
                        stateTime = 0.5f * (float)Game1.Random.NextDouble();

                        break;
                    }
                    AnimateWalking(gT);
                    break;

                #endregion

                #region Move Away - MoveAway the Player, for 

                case RolentoEnemyRangedState.MoveAway:

                    // Are we lined up with player
                    bool linedUpXRanged = LinedUpXWithPlayerRanged();
                    bool linedUpYRanged = LinedUpYWithPlayer();

                    if (linedUpXRanged && linedUpYRanged)
                    {
                        if (InLevel.Player1.IsAttackable) // Check if we can throw stone
                        {
                            // Throw Knife Animation
                            texture = Game1.SprRolentoAttacks;
                            frameX = 0;
                            frameY = 5;
                            state = RolentoEnemyRangedState.ThrowStone;
                            SoundManager.PlaySound("ThrowPunch");
                            drawWidth = DRAW_WIDTH_THROW;
                            drawHeight = DRAW_HEIGHT_THROW;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            SetDrawArea();
                            break;
                        }
                        else
                        {
                            GetRetreatTarget();
                        }
                    }
                    AnimateWalking(gT);
                    break;

                #endregion

                #region ThrowKnife

                case RolentoEnemyRangedState.ThrowStone:
                    AnimateThrowRock(gT);
                    break;

                #endregion

                #region Pre Attack
                case RolentoEnemyRangedState.PreAttack:
                    // Am I still lined up with the player
                    if (LinedUpXWithPlayerClose() && LinedUpYWithPlayer())
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

                            // Its time to attack
                            texture = Game1.SprRolentoAttacks;
                            frameX = 0;
                            frameY = 0;
                            drawWidth = D_WIDTH_COMBO1_ATTACK01;
                            drawHeight = D_HEIGHT_COMBO1_ATTACK01;
                            originCharacters = new Vector2((drawWidth / 2), drawHeight);

                            SetDrawArea();
                            state = RolentoEnemyRangedState.Attack;
                            SoundManager.PlaySound("RolentoThrow");
                            SoundManager.PlaySound("RolentoLaugh2");
                            

                        }
                    }
                    else
                    {
                        // Not lined up with the player
                        state = RolentoEnemyRangedState.MoveTo;
                        frameX = 0;
                        drawWidth = DRAW_WIDTH_WALK;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        return;
                    }
                    AnimateIdle(gT);
                    break;
                #endregion

                #region Attacks

                case RolentoEnemyRangedState.Attack:

                    AnimateKnockDownAttack(gT);
                    break;

                #endregion

                #region Take Hit and Die Cycle
                case RolentoEnemyRangedState.TakeHit:
                    originCharacters = new Vector2((drawWidth / 2), drawHeight);
                    AnimateTakeHit(gT);
                    break;

                case RolentoEnemyRangedState.TakeHitLowKick:
                    originCharacters = new Vector2((drawWidth / 2), drawHeight);
                    AnimateTakeHitKick(gT);
                    break;

                case RolentoEnemyRangedState.KnockedDown:
                    //drawWidth = DRAW_WIDTH_KNOCKDOWN;
                    originCharacters = new Vector2((drawWidth / 2), drawHeight);
                    AnimateKnockDown(gT);
                    break;

                case RolentoEnemyRangedState.Down:
                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime >= Actor.DOWN_TIME)
                    {
                        // Set up Gettign Up Animation
                        state = RolentoEnemyRangedState.GettingUp;
                        texture = Game1.SprRolentoWalkIdle;
                        currentFrameTime = 0f;
                        frameX = 0;
                        frameY = 6;
                        drawWidth = DRAW_WIDTH_GETTINGUP;
                        drawHeight = DRAW_HEIGHT_GETTINGUP;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        SetDrawArea();


                    }
                    break;

                case RolentoEnemyRangedState.GettingUp:
                    AnimateGettingUp(gT);
                    break;

                case RolentoEnemyRangedState.Dying:
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
                            if (Game1.Random.NextDouble() >= 0.1f) // 0.8 = 80% chance of dropping item
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

            }
        }

        public override void Draw(SpriteBatch SB)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);
            GetLayerDepth();

            if (this.IsVisible)
            {
                // Facing Left or Right?
                if (FacingDir == DirectionTarget.Right)
                    SB.Draw(texture, drawPos, drawArea,
                        drawColor, 0f, originCharacters, 1f, SpriteEffects.None, LayerDepth);
                else // We must be facing to the left
                    SB.Draw(texture, drawPos, drawArea,
                        drawColor, 0f, originCharacters, 1f, SpriteEffects.FlipHorizontally, LayerDepth);
            }

            #region HealthBar
            // Red Health Bar
            SB.Draw(Game1.SprSinglePixel, new Vector2(drawPos.X - HEALTH_BAR_WIDTH / 2, drawPos.Y - DRAW_HEIGHT_NORMAL - HEALTH_BAR_HEIGHT),
                new Rectangle(0, 0, HEALTH_BAR_WIDTH + 2, HEALTH_BAR_HEIGHT + 2), new Color(Color.Red, 0.4f), 0f,
                Vector2.Zero, 1f, SpriteEffects.None, this.LayerDepth + 0.001f);

            // How long do we draw the Enemy's Health Bar
            float percent = this.Health / STARTING_HEALTH;
            int drawWidth = (int)(percent * HEALTH_BAR_WIDTH);

            // Yellow Health Bar
            SB.Draw(Game1.SprSinglePixel, new Vector2(drawPos.X - HEALTH_BAR_WIDTH / 2 + 1, drawPos.Y - DRAW_HEIGHT_NORMAL - HEALTH_BAR_HEIGHT + 1),
    new Rectangle(0, 0, drawWidth, HEALTH_BAR_HEIGHT), new Color(Color.Yellow, 0.4f), 0f,
    Vector2.Zero, 1f, SpriteEffects.None, this.LayerDepth);

            #endregion

           // Draw STATE text
           //SB.DrawString(Game1.FontSmall, IsAttacking.ToString(), drawPos, Color.Black, 0f, Vector2.Zero, 0.5f,
           //     SpriteEffects.None, LayerDepth);
            
            // Actor Shadow
            base.Draw(SB);
        }

        public override void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);

            // Facing Left or Right?
            if (FacingDir == DirectionTarget.Right)
                SB.Draw(texture, new Vector2(drawPos.X-10, drawPos.Y), drawArea,
                    drawColor, 0f, originCharacters, 1f, SpriteEffects.None, layerDepth);
            else // We must be facing to the left
                SB.Draw(texture, new Vector2(drawPos.X-10, drawPos.Y), drawArea,
                    drawColor, 0f, originCharacters, 1f, SpriteEffects.FlipHorizontally, layerDepth);

        }

        #region AI Methods

        /// <summary>
        /// Resets player to it's idle state, awaiting player input
        /// </summary>
        private void ResetIdleGraphic()
        {

            this.IsAttacking = false;
            texture = Game1.SprRolentoWalkIdle;
            IsAttackable = true;

            currentFrameTime = 0f;
            frameX = 0;
            frameY = 0;
            drawWidth = DRAW_WIDTH_IDLE;
            drawHeight = DRAW_HEIGHT_NORMAL;
            SetDrawArea();
            originCharacters = new Vector2(drawWidth / 2, drawHeight);

            this.HitArea = drawWidth / 2;

            if (InLevel.Player1 != null)
                DecideWhatToDo();

        }

        private void DecideWhatToDo()
        {
        
            if (Game1.Random.NextDouble() < 0.5d)
            {
                // Decide to retreat
                GetRetreatTarget();

                // Set time to be in Retreat State
                stateTime = (float)(Game1.Random.NextDouble() + 1.8);
            }
            else
            {
                this.state = RolentoEnemyRangedState.MoveAway;
                frameX = 0;
                drawWidth = DRAW_WIDTH_WALK;
            }
        }
        private void GetRetreatTarget()
        {
            state = RolentoEnemyRangedState.Retreat;

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

        private bool LinedUpXWithPlayerRanged()
        {
            // is the player to the left or right
            if (this.Position.X < InLevel.Player1.Position.X) // Player is to the left
            {
                // If far enouggh away
                if (this.Position.X < InLevel.Player1.Position.X - THROW_STONE_SAFE_DISTANCE)
                {
                    return true;
                }
                else
                {
                    // Move left a bit, get more distance
                    this.Position.X -= ACTOR_X_SPEED;
                    FacingDir = DirectionTarget.Right;

                    // Did this movement just take 'this' OFFSCREEN?
                    if (this.Position.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                        this.state = RolentoEnemyRangedState.MoveTo;

                    return false;
                }
                
            }  // is the player to the left or right
            else if (this.Position.X >= InLevel.Player1.Position.X) // Player is to the right
            {
                // If far enouggh away
                if (this.Position.X > InLevel.Player1.Position.X + THROW_STONE_SAFE_DISTANCE)
                {
                    return true;
                }
                else
                {
                    // Move Right a bit, get more distance
                    this.Position.X += ACTOR_X_SPEED;
                    FacingDir = DirectionTarget.Left;

                    // Did this movement just take 'this' OFFSCREEN?
                    if (this.Position.X > Camera.Position.X + Game1.SCREEN_WIDTH / 2)
                        this.state = RolentoEnemyRangedState.MoveTo;


                    return false;
                }

            }
            return false;
        }

        private bool LinedUpXWithPlayerClose()
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

        private void ThrowRocks()
        {
            // Throw a rock 
            this.InLevel.GameItems.Add(new Rock(
            this.Position, this.FacingDir, this.InLevel, this));

            // Play Rock Throw Sound
            SoundManager.PlaySound("RolentoThrow");


        }
        #endregion

        #region Collision Detections Methods
        public override void UpdateHitArea()
        {
            HitArea = drawWidth / 2;
        }

        public override void GetHit(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;

            this.Health -= damage;
            if (CheckForDeath())
            {
                SoundManager.PlaySound("RolentoDye");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            state = RolentoEnemyRangedState.TakeHit;
            texture = Game1.SprRolentoWalkIdle;
            frameX = 0;
            frameY = 3;
            drawWidth = DRAW_WIDTH_TAKEHIT;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("RolentoGetHit");

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
                SoundManager.PlaySound("RolentoDye");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            state = RolentoEnemyRangedState.TakeHitLowKick;
            texture = Game1.SprRolentoWalkIdle;
            frameX = 0;
            frameY = 4;
            drawWidth = DRAW_WIDTH_TAKEHITLOW;
            drawHeight = DRAW_HEIGHT_TAKEHITLOW;
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("RolentoGetHit");

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left)
                FacingDir = DirectionTarget.Right;
            else
                FacingDir = DirectionTarget.Left;

        }

        public override void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
            // Set state and texture
            IsAttackable = false;
            this.IsAttacking = false;
            state = RolentoEnemyRangedState.KnockedDown;
            texture = Game1.SprRolentoWalkIdle;
            frameX = 3;
            frameY = 4;
            drawWidth = DRAW_WIDTH_KNOCKDOWN;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            originCharacters = new Vector2((drawWidth / 2), drawHeight);
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("RolentoGetHit02");

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
                                        // GREAT HIT THEM!
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
                                        // GREAT HIT THEM!
                                        HitSomeone(actor);
                                    }

                                }
                            }
                        }
                    }

                }
            }
        }

        private void SetEffect(string effect, Actor who, RolentoEnemy p)
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, who.Position, InLevel, p);
            InLevel.GameItems.Add(spark);
        }


        private void HitSomeone(Actor whoToHit)
        {
            //SetEffect("smallspark", whoToHit, this);   
            whoToHit.GetKnockedDown(FacingDir, 30);

        }

        #endregion

        #region Animations

        private void SetDrawArea()
        {
            drawArea = new Rectangle(frameX * drawWidth, frameY * drawHeight, drawWidth, drawHeight);
        }

        private void AnimateIdle(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.9f;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 10 && frameY == 0)
                {
                    frameX = 0;
                    frameY++;
                }
                if (frameX > 4)
                {
                    frameX = 0;
                    frameY = 0;
                }

                SetDrawArea();

            }
        }

        private void AnimateTakeHit(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 3)
                {
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }
        private void AnimateTakeHitKick(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 3)
                {
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }
        private void AnimateKnockDown(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 1.2f)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX == 4 && CheckForDeath() && frameY == 4)
                {
                    SoundManager.PlaySound("RolentoDye");
                }
                if (frameX > 5 && frameY == 5)
                {
                    if (CheckForDeath())
                    {
                        //SoundManager.PlaySound("GetHit-Died");
                        state = RolentoEnemyRangedState.Dying;
                        stateTime = 1f;
                        frameX = 4;
                        return;
                    }
                    // Set state and texture
                    state = RolentoEnemyRangedState.Down;
                    frameX = 4;
                    stateTime = 0f;
                    return;
                }
                if (frameX > 6 && frameY == 4)
                {
                    frameX = 0;
                    frameY++;
                }
                SetDrawArea();

            }
        }
        private void AnimateGettingUp(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            float framePerSec = 1.5f;

            if (currentFrameTime >= Actor.FrameRate * framePerSec)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 6 && frameY == 6)
                {
                    frameX = 0;
                    frameY++;
                }

                if (frameX > 1 && frameY == 7)
                {
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }

        private void AnimateWalking(GameTime gT)
        {
            texture = Game1.SprRolentoWalkIdle;
            drawWidth = DRAW_WIDTH_WALK;
            drawHeight = DRAW_HEIGHT_NORMAL;

            originCharacters = new Vector2(drawWidth / 2, drawHeight);

            // Just in case we are coming from an animation which doesn't have frameY
            // set to 1, or 2
            // if (frameY != 1 && frameY != 2)
            frameY = 2;

            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0;
                frameX++;

                if (frameY == 2)
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                    }
                }
                else // FrameY must be equal to 2
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                    }
                }
                SetDrawArea();

            }


        }

        // Enemy Range Attack Animations ( Side Kick )
        private void AnimateKnockDownAttack(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 5 && frameY == 0)
                {
                    frameX = 0;
                    frameY++;
                }

                if (frameX > 4 && frameY == 1)
                {
                    //frameX = 0;
                    ResetIdleGraphic();
                    return; 
                }
                // Collision Detection
                if (frameX == 5 && frameY == 0)
                    CheckPlayerCollision();

                SetDrawArea();

            }
        }

        private void AnimateThrowRock(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *1.4f; // Speed
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 5)
                {
                    texture = Game1.SprRolentoWalkIdle;
                    currentFrameTime = 0f;
                    frameX = 0;
                    frameY = 2;
                    drawWidth = DRAW_WIDTH_WALK;
                    drawHeight = DRAW_HEIGHT_NORMAL;
                    SetDrawArea();
                    originCharacters = new Vector2(drawWidth / 2, drawHeight);
                    GetRetreatTarget();
                    return;
                }
                // Do we throw rocks
                if (frameX == 3)
                {
                    ThrowRocks();
                }
                SetDrawArea();
            }

        }

        #endregion


    }
}
