using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using GravityShift.Import_Code;
using GravityShift.Game_Objects.Static_Objects.Triggers;
using GravityShift.MISC_Code;

namespace GravityShift
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GravityShiftMain : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager mGraphics;
        SpriteBatch mSpriteBatch;

        // Scale - Used to make sure the HUD is drawn based on the screen size
        public Matrix scale;

        Title mTitle;

        //Instance of the Menu class
        Menu mMenu;

        //Instance of the scoring class
        Scoring mScoring;

        //Instance of the level selection class
        LevelSelect mLevelSelect;

        //Instance of the pause class
        Pause mPause;
		
        private GameStates mCurrentState = GameStates.Title;

        //Max duration of a sequence
        private static int VICTORY_DURATION = 120;
        private static int DEATH_DURATION = 50;

        //Duration of a sequence
        private int mSequence = 0;

        //Boolean toggle variable
        private bool mToggledSequence = false;

        private IControlScheme mControls;

        //Current level
        Level mCurrentLevel;

        //Fonts for this game
        SpriteFont mDefaultFont;

        //TO BE CHANGED- Actually, this may be ok since we use this to play test.
        public string LevelLocation { get { return mLevelLocation; } set { mLevelLocation = "..\\..\\..\\Content\\Levels\\" + value; } }        
        private string mLevelLocation = "..\\..\\..\\Content\\Levels\\DefaultLevel.xml";

        public GravityShiftMain()
        {

            Components.Add(new GamerServicesComponent(this));
            mGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

#if XBOX360
            mControls = new ControllerControl();
#else
            if (GamePad.GetState(PlayerIndex.One).IsConnected || GamePad.GetState(PlayerIndex.Two).IsConnected ||
                GamePad.GetState(PlayerIndex.Three).IsConnected || GamePad.GetState(PlayerIndex.Four).IsConnected)
                mControls = new ControllerControl();
            else
                mControls = new KeyboardControl();

#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it c5an query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //COMMENT OUT AFTER TESTING TRIAL MODE
            //Guide.SimulateTrialMode = false;

            mGraphics.PreferredBackBufferWidth = 1280;
            mGraphics.PreferredBackBufferHeight = 720;
            //mGraphics.ToggleFullScreen();// REMEMBER TO RESET AFTER DEBUGGING!!!!!!!!!
            mGraphics.ApplyChanges();

<<<<<<< HEAD
            mTitle = new Title(mControls);
            mMenu = new Menu(mControls);
=======
            mMenu = new Menu(mControls, mGraphics);
>>>>>>> upstream/master
            mScoring = new Scoring(mControls);
            mLevelSelect = new LevelSelect(mControls);
            mPause = new Pause(mControls);

            mSpriteBatch = new SpriteBatch(mGraphics.GraphicsDevice);
            base.Initialize();
		}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // current viewport
            float screenscaleX =
                (float)mGraphics.GraphicsDevice.Viewport.Width / 1280.0f;
            float screenscaleY = (float)mGraphics.GraphicsDevice.Viewport.Height / 720.0f;
            // Create the scale transform for Draw. 
            // Do not scale the sprite depth (Z=1).
            scale = Matrix.CreateScale(screenscaleX, screenscaleY, 1);

            //mGraphics.PreferredBackBufferWidth = 1280;
            //mGraphics.PreferredBackBufferHeight = 720;
            //mGraphics.ApplyChanges();

            mTitle.Load(Content, mGraphics.GraphicsDevice);
            mMenu.Load(Content, mGraphics.GraphicsDevice);
            mScoring.Load(Content);
            mPause.Load(Content);
            GameSound.Load(Content);
            mLevelSelect.Load(Content, mGraphics.GraphicsDevice);
            mCurrentLevel = new Level(mLevelLocation, mControls, GraphicsDevice.Viewport);
            mCurrentLevel.Load(Content);

            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);

            mDefaultFont = Content.Load<SpriteFont>("Fonts/Kootenay");
    }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {}

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (mLevelSelect.TrialMode != Guide.IsTrialMode)
            {
                mLevelSelect.Save();
                mLevelSelect.TrialMode = Guide.IsTrialMode;
                mLevelSelect.Reload();
            }
            

            //if (mGraphics.PreferredBackBufferWidth != 1280 || mGraphics.PreferredBackBufferHeight != 720)
            //{
            //    mGraphics.PreferredBackBufferWidth = 1280;
            //    mGraphics.PreferredBackBufferHeight = 720;
            //    mGraphics.ApplyChanges();
            //}
            // Allows the game to exit
            if (mCurrentState == GameStates.Main_Menu && mControls.isBackPressed(false))
            {
                mLevelSelect.Save();
                if (mControls.controlScheme() == ControlSchemes.Gamepad)
                {

                    if (Guide.IsTrialMode && !Guide.IsVisible)
                    {
                        if (Gamer.SignedInGamers[((ControllerControl)mControls).ControllerIndex] != null)
                        {
                            if (Gamer.SignedInGamers[((ControllerControl)mControls).ControllerIndex].IsSignedInToLive)
                            {
                                if (Gamer.SignedInGamers[((ControllerControl)mControls).ControllerIndex].Privileges.AllowPurchaseContent)
                                {
                                    Guide.ShowMarketplace(((ControllerControl)mControls).ControllerIndex);
                                }
                            }
                        }

                    }

                }
                //if(Gamer.SignedInGamers[PlayerIndex.One].Privileges.AllowPurchaseContent)
                this.Exit();

            }

            if (mCurrentState == GameStates.Title)
            {
                //Check for mute
                GameSound.menuMusic_title.Volume = GameSound.volume;

                //If the correct music isn't already playing
                if (GameSound.menuMusic_title.State != SoundState.Playing)
                    GameSound.StopOthersAndPlay(GameSound.menuMusic_title);

                mTitle.Update(gameTime, ref mCurrentState);

            }

            if (mCurrentState == GameStates.In_Game)
            {
                //Check for mute - not featured yet
                //GameSound.music_level00.Volume = GameSound.volume;


                //If the correct music isn't already playing
                //if (GameSound.music_level00.State != SoundState.Playing)
                //    GameSound.StopOthersAndPlay(GameSound.music_level00);
                
                mCurrentLevel.Update(gameTime, ref mCurrentState);
            }
            else if (mCurrentState == GameStates.Main_Menu)
            {
                //Check for mute
                GameSound.menuMusic_title.Volume = GameSound.volume;

                //If the correct music isn't already playing
                if (GameSound.menuMusic_title.State != SoundState.Playing)
                    GameSound.StopOthersAndPlay(GameSound.menuMusic_title);

                mMenu.Update(gameTime, ref mCurrentState);
            }
            else if (mCurrentState == GameStates.Score)
            {
                //Check for mute
                GameSound.menuMusic_title.Volume = GameSound.volume;
                GameSound.level_stageVictory.Volume = GameSound.volume * .75f;

                //First play win, then menu
                if (GameSound.level_stageVictory.State != SoundState.Playing)
                    if (GameSound.menuMusic_title.State != SoundState.Playing)
                        GameSound.StopOthersAndPlay(GameSound.menuMusic_title);

                

                mScoring.Update(gameTime, ref mCurrentState, ref mCurrentLevel);
            }
            else if (mCurrentState == GameStates.Level_Selection)
            {
                //Check for mute
                GameSound.menuMusic_title.Volume = GameSound.volume;

                //If the correct music isn't already playing
                if (GameSound.menuMusic_title.State != SoundState.Playing)
                    GameSound.StopOthersAndPlay(GameSound.menuMusic_title);

                mLevelSelect.Update(gameTime, ref mCurrentState, ref mCurrentLevel);
            }
            else if (mCurrentState == GameStates.New_Level_Selection)
            {
                //Check for mute
                GameSound.menuMusic_title.Volume = GameSound.volume;

                //If the correct music isn't already playing
                if (GameSound.menuMusic_title.State != SoundState.Playing)
                    GameSound.StopOthersAndPlay(GameSound.menuMusic_title);

                mCurrentLevel = mLevelSelect.Reset();

                mCurrentState = GameStates.Level_Selection;

                mLevelSelect.Update(gameTime, ref mCurrentState, ref mCurrentLevel);

            }
            else if (mCurrentState == GameStates.Pause)
            {
                mPause.Update(gameTime, ref mCurrentState, ref mCurrentLevel);
            }
            else if (mCurrentState == GameStates.Unlock)
            {
                mLevelSelect.UnlockNextLevel();
                mSequence = VICTORY_DURATION;
                mCurrentState = GameStates.Victory;
            }
            else if (mCurrentState == GameStates.Next_Level)
            {
                Level tempLevel = mLevelSelect.GetNextLevel();
                if (tempLevel != null)
                {
                    mCurrentLevel = tempLevel;
                    mCurrentLevel.Load(Content);
                    mCurrentState = GameStates.In_Game;
                }
                else
                    mCurrentState = GameStates.Level_Selection;
            }
            else if (mCurrentState == GameStates.Victory)
            {
                mSequence--;

                if (mSequence <= 0)
                    mCurrentState = GameStates.Score;
            }
            else if (mCurrentState == GameStates.Death)
            {
                if (!mToggledSequence)
                {
                    mSequence = DEATH_DURATION;
                    mToggledSequence = true;
                }
                mSequence--;

                if (mSequence <= 0)
                {
                    mCurrentState = GameStates.In_Game;
                    mToggledSequence = false;
                }
            }
        }

        /// <summary>
        /// Disables the menu from showing initially
        /// </summary>
        public void DisableMenu()
        {
            mCurrentState = GameStates.In_Game;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (mCurrentState == GameStates.In_Game)
            {
                mCurrentLevel.Draw(mSpriteBatch, gameTime, scale);
                mCurrentLevel.DrawHud(mSpriteBatch, gameTime, scale);
            }
            else if (mCurrentState == GameStates.Main_Menu)
                mMenu.Draw(mSpriteBatch, mGraphics, scale);
            
            else if (mCurrentState == GameStates.Score)
                mScoring.Draw(mSpriteBatch, mGraphics, scale);
            else if (mCurrentState == GameStates.Level_Selection)
                mLevelSelect.Draw(mSpriteBatch, mGraphics, scale);
            else if (mCurrentState == GameStates.Pause)
            {
                mCurrentLevel.Draw(mSpriteBatch, gameTime, scale);
                mPause.Draw(mSpriteBatch, mGraphics, scale);
            }
            else if (mCurrentState == GameStates.Victory)
            {
                //TODO - Change this to a victory animation
                mCurrentLevel.Draw(mSpriteBatch, gameTime, scale);
            }
            else if (mCurrentState == GameStates.Death)
            {
                //TODO - Change this to a death animation
                mCurrentLevel.Draw(mSpriteBatch, gameTime, scale);
                mCurrentLevel.DrawHud(mSpriteBatch, gameTime, scale);
            }
            else if (mCurrentState == GameStates.Title)
            {
                mTitle.Draw(mSpriteBatch, gameTime, scale);
            }
                
            base.Draw(gameTime);
        }
    }
}
