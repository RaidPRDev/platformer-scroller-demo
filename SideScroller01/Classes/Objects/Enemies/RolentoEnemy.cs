using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SideScroller01.Classes.Objects.Items;
using SideScroller01.Classes.Scene;

namespace SideScroller01.Classes.Objects.Enemies
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

        // Movement 
        private float stateTime;
        private Vector2 retreatTarget;

        public RolentoEnemy(Vector2 position, Level inLevel)
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
        public RolentoEnemy(DirectionTarget startingSide, Level inLevel)
        : base(Vector2.Zero, inLevel)
        {
            this.Health = STARTING_HEALTH;
            this.FacingDir = startingSide;

            ResetIdleGraphic();
        }

        public RolentoEnemy(DirectionTarget startingSide, Vector2 position, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.Health = STARTING_HEALTH;
            this.FacingDir = startingSide;
            
            ResetIdleGraphic();
        }

        protected override void InitializeTextures()
        {
            base.InitializeTextures();

            Texture_Walk_Idle = TextureManager.GetTexure("RolentoWalkIdleSpriteSheet");
            Texture_Attack = TextureManager.GetTexure("RolentoAttackSpriteSheet");
        }

        public override void Update(GameTime gT)
        {
            switch (this.state)
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
                            this.InitSpriteFrames(DRAW_WIDTH_IDLE, this.DrawHeight, this.FrameX, 0);
                            this.SetOriginPosition("bottom");
                            AnimateIdle(gT);
                        }
                        else
                        {
                            // Not at location
                            this.InitSpriteFrames(DRAW_WIDTH_WALK, DRAW_HEIGHT_NORMAL, this.FrameX, 2);
                            this.SetOriginPosition("bottom");
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
                        this.InitSpriteFrames(DRAW_WIDTH_IDLE, this.DrawHeight, 0, 0);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();
                        this.state = RolentoEnemyRangedState.PreAttack;

                        // How long do we stay in the pre-attack state
                        this.stateTime = 0.5f * (float)Game1.Random.NextDouble();
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
                            this.Sprite = Texture_Attack;
                            this.InitSpriteFrames(DRAW_WIDTH_THROW, DRAW_HEIGHT_THROW, 0, 5);
                            this.SetOriginPosition("bottom");
                            this.SetDrawArea();
                            this.state = RolentoEnemyRangedState.ThrowStone;
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
                        this.stateTime -= (float)gT.ElapsedGameTime.TotalSeconds;
                        if (this.stateTime < 0)
                        {
                            // Is Player Attackable
                            if (!InLevel.Player1.IsAttackable)
                            {
                                GetRetreatTarget();
                                break;
                            }

                            // Its time to attack
                            this.state = RolentoEnemyRangedState.Attack;
                            this.Sprite = Texture_Attack;
                            this.InitSpriteFrames(D_WIDTH_COMBO1_ATTACK01, D_HEIGHT_COMBO1_ATTACK01, 0, 0);
                            this.SetOriginPosition("bottom");
                            this.SetDrawArea();
                            
                            SoundManager.PlaySound("RolentoThrow");
                            SoundManager.PlaySound("RolentoLaugh2");
                        }
                    }
                    else
                    {
                        // Not lined up with the player
                        this.state = RolentoEnemyRangedState.MoveTo;
                        this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
                        this.SetOriginPosition("bottom");
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
                    this.SetOriginPosition("bottom");
                    AnimateTakeHit(gT);
                    break;

                case RolentoEnemyRangedState.TakeHitLowKick:
                    this.SetOriginPosition("bottom");
                    AnimateTakeHitKick(gT);
                    break;

                case RolentoEnemyRangedState.KnockedDown:
                    this.SetOriginPosition("bottom");
                    AnimateKnockDown(gT);
                    break;

                case RolentoEnemyRangedState.Down:
                    this.stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (this.stateTime >= Actor.DOWN_TIME)
                    {
                        // Set up Gettign Up Animation
                        this.state = RolentoEnemyRangedState.GettingUp;
                        this.Sprite = Texture_Walk_Idle;
                        this.CurrentFrameTime = 0;
                        this.InitSpriteFrames(DRAW_WIDTH_GETTINGUP, DRAW_HEIGHT_GETTINGUP, 0, 6);
                        this.SetOriginPosition("bottom");
                        this.SetDrawArea();
                    }
                    break;

                case RolentoEnemyRangedState.GettingUp:
                    AnimateGettingUp(gT);
                    break;

                case RolentoEnemyRangedState.Dying:
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
                            if (Game1.Random.NextDouble() >= 0.1f) // 0.8 = 80% chance of dropping item
                            {
                                this.InLevel.GameItems.Add(new PickUpStone(this.InLevel, this.Position));
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
            this.GetLayerDepth(this.Position.Y);

            if (this.IsVisible)
            {
                // Facing Left or Right?
                if (FacingDir == DirectionTarget.Right)
                    SB.Draw(this.Sprite, drawPos, this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.None, LayerDepth);
                else // We must be facing to the left
                    SB.Draw(this.Sprite, drawPos, this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, SpriteEffects.FlipHorizontally, LayerDepth);
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

            // Draw STATE text
            SB.DrawString(Game1.FontSmall, IsAttacking.ToString(), drawPos, Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, LayerDepth);
            #endregion

            base.Draw(SB);
        }

        public override void DrawInDoorway(SpriteBatch SB, float layerDepth)
        {
            Vector2 drawPos = Camera.GetScreenPosition(Position);
            SpriteEffects sprEffect = SpriteEffects.None;

            // Facing Left or Right?
            if (FacingDir == DirectionTarget.Right) sprEffect = SpriteEffects.None;
            else sprEffect = SpriteEffects.FlipHorizontally;

            SB.Draw(this.Sprite, new Vector2(drawPos.X - 10, drawPos.Y), this.DrawArea, this.DrawColor, 0f, this.Origin, 1f, sprEffect, layerDepth);
        }

        #region AI Methods

        /// <summary>
        /// Resets player to it's idle state, awaiting player input
        /// </summary>
        private void ResetIdleGraphic()
        {
            this.IsAttacking = false;
            this.IsAttackable = true;

            this.Sprite = Texture_Walk_Idle;
            this.CurrentFrameTime = 0;
            this.InitSpriteFrames(DRAW_WIDTH_IDLE, DRAW_HEIGHT_NORMAL, 0, 0);
            this.SetDrawArea();
            this.SetOriginPosition("bottom");
            this.UpdateHitArea();

            if (InLevel.Player1 != null) DecideWhatToDo();
        }

        private void DecideWhatToDo()
        {
            if (Game1.Random.NextDouble() < 0.5d)
            {
                // Decide to retreat
                GetRetreatTarget();

                // Set time to be in Retreat State
                this.stateTime = (float)(Game1.Random.NextDouble() + 1.8);
            }
            else
            {
                this.state = RolentoEnemyRangedState.MoveAway;
                this.InitSpriteFrames(DRAW_WIDTH_WALK, this.DrawHeight, 0, this.FrameY);
            }
        }
        private void GetRetreatTarget()
        {
            this.state = RolentoEnemyRangedState.Retreat;

            // Retreat to which side of the player
            if (Game1.Random.NextDouble() < 0.5d)
            {
                // Go LEFT of the player
                this.retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X - 200),
                    (int)(InLevel.Player1.Position.X - 100));

                // Is this position off screen
                if (this.retreatTarget.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                {
                    // go to the Right Side of Player
                    this.retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X + 100),
                        (int)(InLevel.Player1.Position.X + 200));
                }
            }
            else
            {
                // go to the Right Side of Player
                this.retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X + 100),
                    (int)(InLevel.Player1.Position.X + 200));

                // Is this position off screen
                if (this.retreatTarget.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                {
                    // Go LEFT of the player
                    this.retreatTarget.X = Game1.Random.Next((int)(InLevel.Player1.Position.X - 200),
                        (int)(InLevel.Player1.Position.X - 100));
                }
            }

            // Get Y Retreat Target
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
        public override void GetHit(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;
            this.Health -= damage;

            if (this.CheckForDeath())
            {
                SoundManager.PlaySound("RolentoDye");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            this.state = RolentoEnemyRangedState.TakeHit;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHIT, DRAW_HEIGHT_TAKEHIT, 0, 3);
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left) FacingDir = DirectionTarget.Right;
            else FacingDir = DirectionTarget.Left;

            SoundManager.PlaySound("RolentoGetHit");
        }

        public override void GetHitKick(DirectionTarget cameFrom, int damage)
        {
            this.IsAttacking = false;
            this.Health -= damage;

            if (this.CheckForDeath())
            {
                SoundManager.PlaySound("RolentoDye");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            this.state = RolentoEnemyRangedState.TakeHitLowKick;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_TAKEHITLOW, DRAW_HEIGHT_TAKEHITLOW, 0, 4);
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Left) FacingDir = DirectionTarget.Right;
            else FacingDir = DirectionTarget.Left;

            SoundManager.PlaySound("RolentoGetHit");
        }

        public override void GetKnockedDown(DirectionTarget cameFrom, int damage)
        {
            // Set state and texture
            this.IsAttackable = false;
            this.IsAttacking = false;
            this.Health -= damage;

            this.state = RolentoEnemyRangedState.KnockedDown;
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_KNOCKDOWN, DRAW_HEIGHT_TAKEHIT, 3, 4);
            this.SetOriginPosition("bottom");
            this.SetDrawArea();

            // Face the man who beat you
            if (cameFrom == DirectionTarget.Right) FacingDir = DirectionTarget.Left;
            else FacingDir = DirectionTarget.Right;

            SoundManager.PlaySound("RolentoGetHit02");
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

        private void SetEffect(string effect, Actor who, RolentoEnemy p)
        {
            // Display Small Spark Effect on Hit
            SpecialEffects spark = new SpecialEffects(effect, who.Position, InLevel, p);
            InLevel.GameItems.Add(spark);
        }


        private void HitSomeone(Actor whoToHit)
        {
            whoToHit.GetKnockedDown(FacingDir, 30);
        }

        #endregion

        #region Animations
        private void AnimateIdle(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.9f;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 10 && this.FrameY == 0)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                if (this.FrameX > 4)
                {
                    this.FrameX = 0;
                    this.FrameY = 0;
                }

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
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate * 1.2f)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX == 4 && CheckForDeath() && this.FrameY == 4)
                {
                    SoundManager.PlaySound("RolentoDye");
                }

                if (this.FrameX > 5 && this.FrameY == 5)
                {
                    if (this.CheckForDeath())
                    {
                        this.state = RolentoEnemyRangedState.Dying;
                        this.stateTime = 1f;
                        this.FrameX = 4;
                        return;
                    }
                    // Set state and texture
                    state = RolentoEnemyRangedState.Down;
                    this.FrameX = 4;
                    this.stateTime = 0f;
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

                if (this.FrameX > 6 && this.FrameY == 6)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                if (this.FrameX > 1 && this.FrameY == 7)
                {
                    ResetIdleGraphic();
                    return;
                }

                this.SetDrawArea();
            }
        }

        private void AnimateWalking(GameTime gT)
        {
            this.Sprite = Texture_Walk_Idle;
            this.InitSpriteFrames(DRAW_WIDTH_WALK, DRAW_HEIGHT_NORMAL, this.FrameX, 2);
            this.SetOriginPosition("bottom");

            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0;
                this.FrameX++;

                if (this.FrameY == 2)
                {
                    if (this.FrameX > 9) this.FrameX = 0;
                }
                else // FrameY must be equal to 2
                {
                    if (this.FrameX > 9) this.FrameX = 0;
                }

                this.SetDrawArea();
            }
        }

        // Enemy Range Attack Animations ( Side Kick )
        private void AnimateKnockDownAttack(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 5 && this.FrameY == 0)
                {
                    this.FrameX = 0;
                    this.FrameY++;
                }

                if (this.FrameX > 4 && this.FrameY == 1)
                {
                    ResetIdleGraphic();
                    return; 
                }
                // Collision Detection
                if (this.FrameX == 5 && this.FrameY == 0) this.CheckPlayerCollision();

                this.SetDrawArea();
            }
        }

        private void AnimateThrowRock(GameTime gT)
        {
            this.CurrentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (this.CurrentFrameTime >= Actor.FrameRate)
            {
                this.CurrentFrameTime = 0f;
                this.FrameX++;

                if (this.FrameX > 5)
                {
                    this.Sprite = Texture_Walk_Idle;
                    this.CurrentFrameTime = 0f;
                    this.InitSpriteFrames(DRAW_WIDTH_WALK, DRAW_HEIGHT_NORMAL, 0, 2);
                    this.SetDrawArea();
                    this.SetOriginPosition("bottom");

                    this.GetRetreatTarget();
                    return;
                }
                // Do we throw rocks
                if (this.FrameX == 3) ThrowRocks();

                this.SetDrawArea();
            }
        }
        #endregion
    }
}
