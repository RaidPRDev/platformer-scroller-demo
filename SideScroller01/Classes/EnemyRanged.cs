using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScroller01
{
    enum EnemyRangedState
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

    class EnemyRanged : Actor
    {
        #region constants

        const int HEALTH_BAR_WIDTH = 60;
        const int HEALTH_BAR_HEIGHT = 15;
        const float STARTING_HEALTH = 200;
        const int THROW_STONE_SAFE_DISTANCE = 300;

        #endregion

        #region draw area constants

        // WAIT - Not doing anything, show this one
        private const int DRAW_HEIGHT_WAIT = 122;
        private const int DRAW_WIDTH_WAIT = 96;

        private const int DRAW_HEIGHT_NORMAL = 122;
        private const int DRAW_WIDTH_IDLE = 96;
        private const int DRAW_WIDTH_WALK = 96;
        private const int DRAW_WIDTH_JUMP = 78;
        private const int DRAW_WIDTH_JUMPKICK = 137;

        private const int DRAW_HEIGHT_JUMP = 133;
        private const int DRAW_HEIGHT_JUMPKICK = 133;
        private const int DRAW_WIDTH_JUMPKICK2 = 137;
        private const int DRAW_HEIGHT_JUMPKICK2 = 133;
        private const int DRAW_WIDTH_JUMPPUNCH = 137;
        private const int DRAW_HEIGHT_JUMPPUNCH = 133;

        // Hit Game Item
        private const int DRAW_WIDTH_KICKITEM = 137;
        private const int DRAW_HEIGHT_KICKITEM = 136;

        // Pickup Game Item
        private const int DRAW_WIDTH_PICKUP = 139;
        private const int DRAW_HEIGHT_PICKUP = 133;

        // COMBO 01 - Punch - Punch - Punch
        private const int D_HEIGHT_COMBO1_ATTACK01 = 121;
        private const int D_HEIGHT_COMBO1_ATTACK02 = 121;
        private const int D_HEIGHT_COMBO1_ATTACK03 = 121;
        private const int D_WIDTH_COMBO1_ATTACK01 = 113;
        private const int D_WIDTH_COMBO1_ATTACK02 = 113;
        private const int D_WIDTH_COMBO1_ATTACK03 = 131;
        private const int D_ATTACK01_COMBO1_FRAME_Y = 0;
        private const int D_ATTACK02_COMBO1_FRAME_Y = 1;
        private const int D_ATTACK03_COMBO1_FRAME_Y = 3;

        // COMBO 02 - Kick - Dash Kick
        private const int D_HEIGHT_COMBO2_ATTACK01 = 136;
        private const int D_HEIGHT_COMBO2_ATTACK02 = 136;
        private const int D_HEIGHT_COMBO2_ATTACK03 = 136;
        private const int D_WIDTH_COMBO2_ATTACK01 = 131;
        private const int D_WIDTH_COMBO2_ATTACK02 = 131;
        private const int D_WIDTH_COMBO2_ATTACK03 = 131;
        private const int D_ATTACK01_COMBO2_FRAME_Y = 0;
        private const int D_ATTACK02_COMBO2_FRAME_Y = 0;
        private const int D_ATTACK03_COMBO2_FRAME_Y = 1;

        // Enemy
        private const int DRAW_WIDTH_TAKEHIT = 136;
        private const int DRAW_HEIGHT_TAKEHIT = 133;
        private const int DRAW_WIDTH_GETTINGUP = 137;
        private const int DRAW_WIDTH_KNOCKDOWN = 139;

        // Game Items
        private const int DRAW_WIDTH_ROCKTHROW = 139;

        #endregion

        EnemyRangedState state;

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
        
        public EnemyRanged(Vector2 position, Level inLevel)
            : base(position, inLevel)
        {
            
            ResetIdleGraphic();
            drawColor = new Color(0.2f, 0.2f, 1f); // Set the draw color
            this.Health = STARTING_HEALTH;

        }

               /// <summary>
        /// Create an enemy to spawn on a particular side of the screen
        /// </summary>
        /// <param name="startingSide"></param>
        /// <param name="inLevel"></param>
        public EnemyRanged(DirectionTarget startingSide, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.FacingDir = startingSide;

            ResetIdleGraphic();
            drawColor = new Color(0.2f, 0.2f, 1f); // Set the draw color
            this.Health = STARTING_HEALTH;

        }

        public EnemyRanged(DirectionTarget startingSide, Vector2 position, Level inLevel)
            : base(Vector2.Zero, inLevel)
        {
            this.FacingDir = startingSide;
            
            ResetIdleGraphic();
            drawColor = new Color(0.2f, 0.2f, 1f); // Set the draw color
            this.Health = STARTING_HEALTH;

        }


        public override void Update(GameTime gT)
        {
            switch (state)
            {
                #region Retreat State
                case EnemyRangedState.Retreat:

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
                            this.Position.X += 2f;
                            if (Position.X > retreatTarget.X) // Gone too far
                                Position.X = retreatTarget.X;
                        }
                        else // Target to the LEFT
                        {
                            this.Position.X -= 2f;
                            if (Position.X < retreatTarget.X) // Gone too far
                                Position.X = retreatTarget.X;
                        }
                        // Move to Y Location
                        if (Position.Y < retreatTarget.Y) // target is Below US
                        {
                            this.Position.Y += 1.5f;
                            if (Position.Y > retreatTarget.Y) // Gone too far
                                Position.Y = retreatTarget.Y;
                        }
                        else // Target is above us  
                        {
                            this.Position.Y -= 1.5f;
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
                            //drawHeight = DRAW_HEIGHT_NORMAL; ;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            AnimateIdle(gT);

                        }
                        else
                        {
                            // Not at location
                            frameY = 1;
                            drawWidth = DRAW_WIDTH_WALK;
                            drawHeight = DRAW_HEIGHT_NORMAL;
                            originCharacters = new Vector2(drawWidth / 2, drawHeight);
                            AnimateWalking(gT);

                        }
                    }
                    break;

                #endregion

                #region MoveTo - move to the Player, for attacking
                case EnemyRangedState.MoveTo:
                    // Are we lined up with player
                    bool linedUpX = LinedUpXWithPlayerClose();
                    bool linedUpY = LinedUpYWithPlayer();

                    if (linedUpX && linedUpY)
                    {
                        // Set Pre-Attack State
                        frameX = 0;
                        frameY = 0;
                        state = EnemyRangedState.PreAttack;
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

                case EnemyRangedState.MoveAway:

                    // Are we lined up with player
                    bool linedUpXRanged = LinedUpXWithPlayerRanged();
                    bool linedUpYRanged = LinedUpYWithPlayer();

                    if (linedUpXRanged && linedUpYRanged)
                    {
                        if (InLevel.Player1.IsAttackable) // Check if we can throw stone
                        {
                            // Throw Knife Animation
                            texture = Game1.SprCharacterReact;
                            frameX = 0;
                            frameY = 7;
                            state = EnemyRangedState.ThrowStone;
                            SoundManager.PlaySound("ThrowPunch");
                            drawWidth = DRAW_WIDTH_ROCKTHROW;
                            drawHeight = DRAW_HEIGHT_TAKEHIT;
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

                case EnemyRangedState.ThrowStone:
                    AnimateThrowRock(gT);
                    break;

                #endregion

                #region Pre Attack
                case EnemyRangedState.PreAttack:
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
                            texture = Game1.SprCharacterAttacks;
                            frameX = 0;
                            frameY = 3;
                            drawWidth = D_WIDTH_COMBO1_ATTACK03;
                            drawHeight = D_HEIGHT_COMBO1_ATTACK03;
                            if (FacingDir == DirectionTarget.Left)
                                originCharacters = new Vector2((drawWidth / 2) + 18, drawHeight + 2);
                            else // Right
                                originCharacters = new Vector2((drawWidth / 2) - 18, drawHeight + 2);

                            SetDrawArea();
                            state = EnemyRangedState.Attack;
                            SoundManager.PlaySound("ThrowPunch");

                        }
                    }
                    else
                    {
                        // Not lined up with the player
                        state = EnemyRangedState.MoveTo;
                        frameX = 0;
                        drawWidth = DRAW_WIDTH_WALK;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        return;
                    }
                    AnimateIdle(gT);
                    break;
                #endregion

                #region Attacks

                case EnemyRangedState.Attack:

                    AnimateKnockDownAttack(gT);
                    break;

                #endregion

                #region Take Hit and Die Cycle
                case EnemyRangedState.TakeHit:
                    originCharacters = new Vector2((drawWidth / 2) - 5, drawHeight);
                    AnimateTakeHit(gT);
                    break;

                case EnemyRangedState.TakeHitLowKick:
                    originCharacters = new Vector2((drawWidth / 2) - 5, drawHeight);
                    AnimateTakeHitKick(gT);
                    break;

                case EnemyRangedState.KnockedDown:
                    //drawWidth = DRAW_WIDTH_KNOCKDOWN;
                    //originCharacters = new Vector2((drawWidth / 2) - 5, drawHeight);
                    AnimateKnockDown(gT);
                    break;

                case EnemyRangedState.Down:
                    stateTime += (float)gT.ElapsedGameTime.TotalSeconds;
                    if (stateTime >= Actor.DOWN_TIME)
                    {
                        // Set up Gettign Up Animation
                        state = EnemyRangedState.GettingUp;
                        currentFrameTime = 0f;
                        frameX = 0;
                        frameY = 5;
                        drawWidth = DRAW_WIDTH_GETTINGUP;
                        drawHeight = DRAW_HEIGHT_TAKEHIT;
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        SetDrawArea();


                    }
                    break;

                case EnemyRangedState.GettingUp:
                    AnimateGettingUp(gT);
                    break;

                case EnemyRangedState.Dying:
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
                            if (Game1.Random.NextDouble() >= 0.6f) // 0.6 = 40% chance of dropping item
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
            texture = Game1.SprCharacterWalkIdle;
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
                this.state = EnemyRangedState.MoveAway;
                frameX = 0;
                drawWidth = DRAW_WIDTH_WALK;
            }
        }
        private void GetRetreatTarget()
        {
            state = EnemyRangedState.Retreat;

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
                    this.Position.X -= 2.5f;
                    FacingDir = DirectionTarget.Right;

                    // Did this movement just take 'this' OFFSCREEN?
                    if (this.Position.X < Camera.Position.X - Game1.SCREEN_WIDTH / 2)
                        this.state = EnemyRangedState.MoveTo;

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
                    this.Position.X += 2.5f;
                    FacingDir = DirectionTarget.Left;

                    // Did this movement just take 'this' OFFSCREEN?
                    if (this.Position.X > Camera.Position.X + Game1.SCREEN_WIDTH / 2)
                        this.state = EnemyRangedState.MoveTo;


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
                this.Position.X -= 2.5f;
                FacingDir = DirectionTarget.Left;
                return false;
            }
            else if (InLevel.Player1.Position.X >= this.Position.X + 80) // Player is to the right
            {
                // Move Right
                this.Position.X += 2.5f;
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
                this.Position.Y -= 1.5f;
                return false;
            }
            else if (InLevel.Player1.Position.Y >= this.Position.Y + 8) // Player is BELOW
            {
                // Move enemy down a bit
                this.Position.Y += 1.5f;
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
            SoundManager.PlaySound("TakeThat");


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
                SoundManager.PlaySound("GetHit-Died");
                GetKnockedDown(cameFrom, 0);
                return;
            }

            // Set state and tetxture
            state = EnemyRangedState.TakeHit;
            texture = Game1.SprCharacterReact;
            frameX = 0;
            frameY = 6;
            drawWidth = DRAW_WIDTH_TAKEHIT;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            SetDrawArea();

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
            state = EnemyRangedState.TakeHitLowKick;
            texture = Game1.SprCharacterReact;
            frameX = 3;
            frameY = 6;
            drawWidth = DRAW_WIDTH_TAKEHIT;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            SetDrawArea();

            // Play Sound
            SoundManager.PlaySound("GetHit02");
            SoundManager.PlaySound("GetHit-Shit");

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
            state = EnemyRangedState.KnockedDown;
            texture = Game1.SprCharacterReact;
            frameX = 0;
            frameY = 1;
            drawWidth = DRAW_WIDTH_KNOCKDOWN;
            drawHeight = DRAW_HEIGHT_TAKEHIT;
            originCharacters = new Vector2((drawWidth / 2), drawHeight);
            SetDrawArea();

            // Play Sound
            //SoundManager.PlaySound("MetalSound2");
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

        private void HitSomeone(Actor whoToHit)
        {
            whoToHit.GetKnockedDown(FacingDir, 15);

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
                if (frameX > 7)
                    frameX = 0;

                SetDrawArea();

            }
        }

        private void AnimateTakeHit(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            if (currentFrameTime >= Actor.FrameRate * 1.2f)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 1)
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
            if (currentFrameTime >= Actor.FrameRate * 1.8f)
            {
                currentFrameTime = 0f;
                frameX++;

                if (frameX > 4)
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
                if (frameX == 1 && CheckForDeath())
                {
                    SoundManager.PlaySound("GetHit-Died");
                }
                if (frameX > 7)
                {
                    if (CheckForDeath())
                    {
                        //SoundManager.PlaySound("GetHit-Died");
                        state = EnemyRangedState.Dying;
                        stateTime = 1f;
                        frameX = 6;
                        return;
                    }
                    // Set state and texture
                    state = EnemyRangedState.Down;
                    frameX = 6;
                    stateTime = 0f;
                    return;
                }
                SetDrawArea();

            }
        }
        private void AnimateGettingUp(GameTime gT)
        {
            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;
            float framePerSec = 1.5f;

            //if (frameX == 7) framePerSec = 7f;
            //else framePerSec = 1.5f;
            if (currentFrameTime >= Actor.FrameRate * framePerSec)
            {
                currentFrameTime = 0f;
                frameX++;
                if (frameX > 7)
                {
                    ResetIdleGraphic();
                    return;
                }
                SetDrawArea();

            }
        }

        private void AnimateWalking(GameTime gT)
        {
            texture = Game1.SprCharacterWalkIdle;
            drawWidth = DRAW_WIDTH_WALK;
            drawHeight = DRAW_HEIGHT_NORMAL;

            originCharacters = new Vector2(drawWidth / 2, drawHeight);

            // Just in case we are coming from an animation which doesn't have frameY
            // set to 1, or 2
            // if (frameY != 1 && frameY != 2)
            frameY = 1;

            currentFrameTime += (float)gT.ElapsedGameTime.TotalSeconds;// *.5f; // * float makes it faster or slower
            if (currentFrameTime >= Actor.FrameRate)
            {
                currentFrameTime = 0;
                frameX++;

                if (frameY == 1)
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                        //frameY++;
                    }
                }
                else // FrameY must be equal to 2
                {
                    if (frameX > 9)
                    {
                        frameX = 0;
                        //frameY--;
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
                if (frameX > 6)
                {
                    //frameX = 0;
                    ResetIdleGraphic();
                    return; 
                }
                // Collision Detection
                if (frameX == 4)
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

                if (frameX > 7 && frameY == 7)
                {
                    frameX = 0;
                    frameY++;
                }
                if (frameX > 3 && frameY == 8)
                {
                    frameX = 0;
                    if (frameY == 8)
                    {
                        texture = Game1.SprCharacterWalkIdle;
                        currentFrameTime = 0f;
                        frameX = 0;
                        frameY = 1;
                        drawWidth = DRAW_WIDTH_WALK;
                        drawHeight = DRAW_HEIGHT_NORMAL;
                        SetDrawArea();
                        originCharacters = new Vector2(drawWidth / 2, drawHeight);
                        GetRetreatTarget();
                        return;
                    }
                }
                // Do we throw rocks
                if (frameX == 0 && frameY == 8)
                {
                    ThrowRocks();
                }
                SetDrawArea();
            }

        }

        #endregion





    }
}
