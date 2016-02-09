/*
 * Description:     A basic PONG simulator
 * Author:          Joe S     
 * Date:            February 08 16'
 *
 * TODO - Random Movement? Manipulation of screen? Multiple balls? Power-ups? ---- ROUND Spherical paddles?
 * 9:20 PM Feb 08 16' Note: Need to work on screen manipulation
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region Global Values






        //paddle position variables
        int paddle1Y, paddle2Y;

        // LIFE
        int ballHitCount;

        //ball position variables
        int ballX, ballY;


        // check to see if a new game can be started
        Boolean newGameOk = true;


        //ball directions
        Boolean ballMoveRight = false;
        Boolean ballMoveDown = false;


        //constants used to set size and speed of paddles 
        const int PADDLE_LENGTH = 40;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle
        int paddleSpeed = 8;


        //constants used to set size and speed of ball 
        const int BALL_SIZE = 10;
        int ballSpeed = 8;


        //player scores
        int player1Score = 0;
        int player2Score = 0;


        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;


        //game winning score
        int gameWinScore = 4;


        //brush for paint method
        SolidBrush drawBrush = new SolidBrush(Color.White);

        #endregion

        public Form1()
        {
            InitializeComponent();
            SetParameters();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Check to see if a key is pressed and set its KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                    if (newGameOk == true)
                    {
                        GameStart();
                    }
                    break;
                case Keys.N:
                    if (newGameOk == true)
                    {
                        startLabel.Text = "Aw. You don't like me?";
                        this.Refresh();
                        Thread.Sleep(1000);
                        startLabel.Text = "... Okay. Bye";
                        this.Refresh();
                        Thread.Sleep(1000);
                        startLabel.Text = "... Okay. Bye :(";
                        this.Refresh();
                        Thread.Sleep(1000);


                        Close();
                    }
                    break;
                case Keys.Space:
                    if (newGameOk == true)
                    {
                        GameStart();
                    }
                    break;
                default:
                    break;
            }
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Sets up the game objects in their start position, resets the scores, displays a 
        /// countdown, and then starts the game timer.
        /// </summary>
        private void GameStart()
        {
            newGameOk = true;
            SetParameters();

            SolidBrush decadentBrush = new SolidBrush(Color.Black);
            Font dFont = new Font("Courier New", 26);
            Graphics life = this.CreateGraphics();


            startLabel.Visible = false;
            Refresh();

            //countdown to start of game
            for (int x = 3; x > 0; x--)
            {
                life.DrawString(Convert.ToString(x), dFont, decadentBrush, (this.Width / 2) - 13, (this.Height / 2) - 13);
                Thread.Sleep(1000);
                this.Refresh();
            }

            gameUpdateLoop.Start();
            newGameOk = false;
        }


        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                // sets location of score labels to the middle of the screen
                player1Label.Location = new Point(10, 10);
                player2Label.Location = new Point(this.Width - player2Label.Width - 50, 10);

                //set label, score variables, and ball position
                player1Score = player2Score = 0;
                player1Label.Text = "Player 1:  " + player1Score;
                player2Label.Text = "Player 2:  " + player2Score;

                paddle1Y = paddle2Y = this.Height / 2 - PADDLE_LENGTH / 2;

                ballHitCount = 0;



            }

            paddleSpeed = ballSpeed = 8;
            ballX = (this.Width / 2) - (BALL_SIZE / 2);
            ballY = (this.Height / 2) - (BALL_SIZE / 2);


        }


        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            //sound player to be used for all in game sounds initially set to collision sound
            SoundPlayer player = new SoundPlayer();
            player = new SoundPlayer(Properties.Resources.collision);

            #region update ball position


            if (ballMoveRight == true)
            {
                ballX += ballSpeed;

            }
            else
            {
                ballX -= ballSpeed;
            }


            if (ballMoveDown == true)
            {
                ballY += ballSpeed;
            }
            else
            {
                ballY -= ballSpeed;
            }

            #endregion


            #region update paddle positions

            if (aKeyDown == true && paddle1Y > 0)
            {
                paddle1Y -= paddleSpeed;
            }
            if (zKeyDown == true && paddle1Y + PADDLE_LENGTH < this.Height)
            {
                paddle1Y += paddleSpeed;
            }

            if (jKeyDown == true && paddle2Y > 0)
            {
                paddle2Y -= paddleSpeed;
            }
            if (mKeyDown == true && paddle2Y + PADDLE_LENGTH < this.Height)
            {
                paddle2Y += paddleSpeed;
            }

            #endregion


            #region ball collision with top and bottom lines

            if (ballY < 0) // if ball hits top line
            {
                ballMoveDown = true;
                player.Play();
            }

            if (ballY + BALL_SIZE > this.Height) // if ball hits top line
            {
                ballMoveDown = false;
                player.Play();
            }


            #endregion


            #region ball collision with paddles

            if (ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH && ballX < PADDLE_EDGE + PADDLE_WIDTH) // left paddle collision
            {
                player.Play();
                ballMoveRight = true;
                ballSpeed += 2;
                paddleSpeed += 2;
                ballHitCount++;


                if (this.Height > 200)
                {
                    this.Height -= 5 * ballHitCount;
                    //    this.Width += 4 * ballHitCount;
                }
                #region Screen Manipulation

                //if (ballHitCount >= 1) // EDIT
                //{
                //    Random randNum = new Random();
                //    Random rand2Num = new Random();
                //    Random rand3Num = new Random();


                //    int genie = randNum.Next(20, 81);
                //    int subOrAdd = rand2Num.Next(1, 101);

                //    if (subOrAdd % 2 == 0)
                //    {
                //        int widthOrHeight = rand3Num.Next(1, 101);

                //        if (widthOrHeight == 0)
                //        {
                //            this.Height -= genie;
                //        }
                //        else
                //        {
                //            this.Width -= genie;
                //        }
                //    }
                //    else
                //    {
                //        int widthOrHeight = randNum.Next(1, 101);

                //        if (widthOrHeight == 0)
                //        {
                //            this.Height += genie;
                //        }
                //        else
                //        {
                //            this.Width += genie;
                //        }
                //    }
                //}

                #endregion

            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2) // right paddle collision
            {
                player.Play();
                ballMoveRight = false;
                ballSpeed += 2;
                paddleSpeed += 2;
                ballHitCount++;

                if (this.Height > 200)

                    this.Height -= 5 * ballHitCount;
                //    this.Width += 4 * ballHitCount;
            }
            #region Screen Manipulation

            //if (ballHitCount >= 1) // EDIT
            //{
            //    Random randNum = new Random();
            //    Random rand2Num = new Random();
            //    Random rand3Num = new Random();


            //    int genie = randNum.Next(20, 81);
            //    int subOrAdd = rand2Num.Next(1, 101);

            //    if (subOrAdd % 2 == 0)
            //    {
            //        int widthOrHeight = rand3Num.Next(1, 101);

            //        if (widthOrHeight == 0)
            //        {
            //            this.Height -= genie;
            //        }
            //        else
            //        {
            //            this.Width -= genie;
            //        }
            //    }
            //    else
            //    {
            //        int widthOrHeight = randNum.Next(1, 101);

            //        if (widthOrHeight == 0)
            //        {
            //            this.Height += genie;
            //        }
            //        else
            //        {
            //            this.Width += genie;
            //        }
            //    }
            //}

            #endregion*/


            #endregion


            #region ball collision with side walls (point scored)

            player = new SoundPlayer(Properties.Resources.score);

            if (ballX < 0)
            {
                player.Play();
                player2Score++;
                player2Label.Text = "Player 2: " + Convert.ToString(player2Score);
                this.Refresh();

                if (player2Score == gameWinScore)
                {
                    GameOver("Player 1");
                }
                else
                {
                    SetParameters();
                }

            }

            if (ballX + BALL_SIZE > this.Width)
            {
                player.Play();
                player1Score++;
                player1Label.Text = "Player 1: " + Convert.ToString(player1Score);
                this.Refresh();

                if (player1Score == gameWinScore)
                {
                    GameOver("Player 2");
                }
                else
                {
                    SetParameters();
                }

            }

            #endregion


            Refresh();
        }


        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string loser)
        {
            newGameOk = true;

            gameUpdateLoop.Stop();
            startLabel.Text = "The loser is " + loser + "!!";
            startLabel.Visible = true;
            Refresh();
            Thread.Sleep(2000);
            startLabel.Text = "Do you want to play again? Y or N";

        }


        private void closeButton_Click(object sender, EventArgs e)
        {
            startLabel.Text = "Aw. You don't like me?";
            Thread.Sleep(500);
            startLabel.Text = "... Okay. Bye";
            Thread.Sleep(500);
            startLabel.Text = "... Okay. Bye :(";
            Thread.Sleep(500);

            Close();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.FillRectangle(drawBrush, PADDLE_EDGE, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH);
            e.Graphics.FillRectangle(drawBrush, this.Width - PADDLE_EDGE - PADDLE_WIDTH, paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH);

            e.Graphics.FillRectangle(drawBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
        }

    }
}
