using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;

/*********************
 * Name: Rohit Sen
 * Date: 12/2/21
 * Desc: Creates an assistive calculator that can help people with visual disabilities.
*********************/

namespace AssistiveCalculator
{

    public partial class Form1 : Form
    {
        //starting values
        Double val = 0;
        Double previousVal = 0;
        String text = "";
        bool oper_press = false;

        //this keeps track of the total operations done by the calculator
        int totalOperationsDone = 0;

        //supported commands
        String[] supportedCommands = new string[] { "0", "1", "2", "3", "4", "5", "6",
            "7", "8", "9", "plus", "minus", "times", "divided by", "equals", "clear", "help" };

        //defines the voice assistant which will be used to
        //speak when calculations are performed, as well as convert spoken words
        //into actions for the calculator
        SpeechRecognitionEngine voiceAssist = new SpeechRecognitionEngine();
        SpeechSynthesizer speech = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
        }

        private void voiceAssist_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            voiceCalc(e.Result.Text);
        }

        private void speakHelpMessage()
        {
            //iterate through all the commands that this calculator
            //can handle and speak it out
            speech.Speak("Hello, this calculator can handle the following key words");
            for (int i = 0; i < supportedCommands.Length; i++)
            {
                speech.Speak(supportedCommands[i]);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //clears and changes number to 0
            output.Text = "0";
            speech.Speak("Clear!");
        }

        private void button_click(object sender, EventArgs e)
        {
            //starts at zero
            if((output.Text == "0") ||(oper_press))
            {
                output.Clear();
            }

            //inputs number 
            Button button = (Button)sender;
            output.Text = output.Text + button.Text;
            String num = output.Text;
            speech.Speak(num);
            oper_press = false;
        }

        private void op_pres(object sender, EventArgs e)
        {
            //if a value is pressed, then a value is generated on the screen
            Button button = (Button)sender;
            text = button.Text;
            val = Double.Parse(output.Text);
            //adds numbers
            if (text == "+")
            {
                speech.Speak("plus");
            }
            //subtracts numbers
            if (text == "-")
            {
                speech.Speak("minus");
            }
            //multiplies numbers
            if (text == "*")
            {
                speech.Speak("times");
            }
            //divides numbers
            if (text == "/")
            {
                speech.Speak("divided by");
            }
            oper_press = true;
        }

        private void op_res(object sender, EventArgs e)
        {
            switch(text)
            {
                //adds numbers
                case "+":
                    output.Text = (val + Double.Parse(output.Text)).ToString();
                    String num1 = output.Text;
                    speech.Speak(num1);
                    break;
               //subtracts numbers
                case "-":
                    output.Text = (val - Double.Parse(output.Text)).ToString();
                    String num2 = output.Text;
                    speech.Speak(num2);
                    break;
                //multiplies numbers
                case "*":
                    output.Text = (val * Double.Parse(output.Text)).ToString();
                    String num3 = output.Text;
                    speech.Speak(num3);
                    break;
                //divides numbers
                case "/":
                    output.Text = (val / Double.Parse(output.Text)).ToString();
                    String num4 = output.Text;
                    speech.Speak(num4);
                    break;
                default:
                    break;
            }
            oper_press = false;

            //incremental the total operations done
            totalOperationsDone++;
            label1.Text = "Total Calculations: " + totalOperationsDone.ToString();
        }

        private void c_pres(object sender, EventArgs e)
        {
            //clears value
            output.Clear();
            val = 0;

            string num = val.ToString();
            speech.Speak(num);
            oper_press = true;
        }

        private void recordVoice_Click(object sender, EventArgs e)
        {
            //starts the voice recognition
            startVoiceRecognition();
        }

        private void startVoiceRecognition()
        {
            Choices commands = new Choices();

            //an array shown with commands the user can use
            commands.Add(supportedCommands);
            Grammar gr = new Grammar(new GrammarBuilder(commands));

            try
            {
                voiceAssist.RequestRecognizerUpdate();
                voiceAssist.LoadGrammar(gr);
                voiceAssist.SpeechRecognized += voiceAssist_SpeechRecognized;
                //recognize speech from the default audio input in the computer
                voiceAssist.SetInputToDefaultAudioDevice();

                //start listening for words from the user
                voiceAssist.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                return;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Speak the help message when the help button is clicked
            speakHelpMessage();
        }

        private void voiceCalc(String spokenText)
        {
            //process help request
            if (spokenText == "help")
            {
                speakHelpMessage();
            }

            //process numbers
            if (spokenText == "0" ||
                spokenText == "1" ||
                spokenText == "2" ||
                spokenText == "3" ||
                spokenText == "4" ||
                spokenText == "5" ||
                spokenText == "6" ||
                spokenText == "7" ||
                spokenText == "8" ||
                spokenText == "9")
            {
                if (output.Text == "0")
                {
                    //do this so that we don't add 0 at the start of the number
                    output.Text = spokenText;
                }
                else
                {
                    //add the newly entered digit to the existing digits
                    output.Text = output.Text + spokenText;
                }
                speech.Speak(output.Text);

            }

            //process operations
            if (output.Text != "")
            {
                //if the user wants to add numbers
                if (spokenText == "plus")
                {
                    text = "+";
                    speech.Speak("plus");
                    previousVal = Double.Parse(output.Text);
                    output.Text = "0";
                }
                //if the user wants to subtracts numbers
                if (spokenText == "minus")
                {
                    text = "-";
                    speech.Speak("minus");
                    previousVal = Double.Parse(output.Text);
                    output.Text = "0";
                }
                //if the user wants to multiply numbers
                if (spokenText == "times")
                {
                    text = "*";
                    speech.Speak("times");
                    previousVal = Double.Parse(output.Text);
                    output.Text = "0";
                }
                //if the user wants to divide numbers
                if (spokenText == "divided by")
                {
                    text = "/";
                    speech.Speak("divided by");
                    previousVal = Double.Parse(output.Text);
                    output.Text = "0";
                }
            }

            //compute the result
            if (spokenText == "equals")
            {
                val = Double.Parse(output.Text);

                switch (text)
                {
                    //adds numbers
                    case "+":
                        double numAdded = val + previousVal;
                        output.Text = numAdded.ToString();
                        String num1 = output.Text;
                        speech.Speak(num1);
                        break;
                    //subtracts numbers
                    case "-":
                        double numSubtracted = previousVal - val;
                        output.Text = numSubtracted.ToString();
                        String num2 = output.Text;
                        speech.Speak(num2);
                        break;
                    //multiplies numbers
                    case "*":
                        double numMultiplied = val * previousVal;
                        output.Text = numMultiplied.ToString();
                        String num3 = output.Text;
                        speech.Speak(num3);
                        break;
                    //divides numbers
                    case "/":
                        double numDivided = previousVal / val;
                        output.Text = numDivided.ToString();
                        String num4 = output.Text;
                        speech.Speak(num4);
                        break;
                    default:
                        break;
                }

                //incremental the total operations done
                totalOperationsDone++;
                label1.Text = "Total Calculations: " + totalOperationsDone.ToString();
            }
            if (spokenText == "clear")
            {
                //clears and changes number to 0
                output.Text = "0";
                speech.SpeakAsync("Clear!");
            }
        }
    }
}
