﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using analizador_gramaticaunidad1.sql.com.analizador;
using Irony.Parsing;
using System.IO;
using System.Threading;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace analizador_gramaticaunidad1
{
    public partial class PERAcode : Form
    {

        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        List<VoiceInfo> vocesInfo = new List<VoiceInfo>();
        RegexLexer csLexer = new RegexLexer();
        bool errorcodigo = true;
        bool load;
      public static   List<string> vtipo = new List<string>();
       public static List<string> vnombre = new List<string>();
       public static List<string> vvalor = new List<string>();
        public PERAcode()
        {
           
            InitializeComponent();
            
        }
        //
      
        List<string> palabrasReservadas;

        private void Speedtext(String speed)
        {
            synthesizer.SelectVoice("Microsoft Sabina Desktop");
            //MessageBox.Show("volumen "+Volumen+" Rate"+Rate);
            synthesizer.Volume = 100;
            synthesizer.Rate = 0;
            synthesizer.Speak(speed);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            // Add the keywords to the list.
            entrada2.Settings.Keywords.Add("function");
            entrada2.Settings.Keywords.Add("startif");
            entrada2.Settings.Keywords.Add("then");
            entrada2.Settings.Keywords.Add("else");
            entrada2.Settings.Keywords.Add("elseif");
            entrada2.Settings.Keywords.Add("endif");
            entrada2.Settings.Keywords.Add("startfor");
            entrada2.Settings.Keywords.Add("endfor");
            entrada2.Settings.Keywords.Add("int");
            entrada2.Settings.Keywords.Add("double");
            entrada2.Settings.Keywords.Add("String");
            entrada2.Settings.Keywords.Add("public");
            entrada2.Settings.Keywords.Add("class");
            entrada2.Settings.Keywords.Add("static");
            entrada2.Settings.Keywords.Add("void");
            entrada2.Settings.Keywords.Add("main");
            entrada2.Settings.Keywords.Add("import");

            // Set the comment identifier. 
            // For Lua this is two minus-signs after each other (--).
            // For C++ code we would set this property to "//".
            entrada2.Settings.Comment = "//";

            // Set the colors that will be used.
            entrada2.Settings.KeywordColor = Color.Blue;
            entrada2.Settings.CommentColor = Color.Green;
            entrada2.Settings.StringColor = Color.Gray;
            entrada2.Settings.IntegerColor = Color.Red;

            // Let's not process strings and integers.
            entrada2.Settings.EnableStrings = true;
            entrada2.Settings.EnableIntegers = true;

            // Let's make the settings we just set valid by compiling
            // the keywords to a regular expression.
            entrada2.CompileKeywords();

            // Load a file and update the syntax highlighting.
       //     entrada2.LoadFile("script.txt", RichTextBoxStreamType.PlainText);
            entrada2.ProcessAllLines();

            timer1.Interval = 10;
            timer1.Start();
            using (StreamReader sr = new StreamReader(@"..\..\RegexLexer.cs"))
            {
                //tbxCode.Text = sr.ReadToEnd();

                csLexer.AddTokenRule(@"\s+", "ESPACIO", true);
                csLexer.AddTokenRule(@"\b[_a-zA-Z][\w]*\b", "IDENTIFICADOR");
                csLexer.AddTokenRule("\".*?\"", "CADENA");
                csLexer.AddTokenRule(@"'\\.'|'[^\\]'", "CARACTER");
                csLexer.AddTokenRule("//[^\r\n]*", "COMENTARIO1");
                csLexer.AddTokenRule("/[*].*?[*]/", "COMENTARIO2");
                csLexer.AddTokenRule(@"\d*\.?\d+", "NUMERO");
                csLexer.AddTokenRule(@"[\(\)\[\]\:\;,]", "DELIMITADOR");
                csLexer.AddTokenRule(@"[\.\+\-/*%]", "OPERADOR");
                // csLexer.AddTokenRule("[|.|+|-|*|%]", "OPERADOR");
                csLexer.AddTokenRule(">|<|==|>=|<=|!", "COMPARADOR");
                csLexer.AddTokenRule("&&", "DOBLECONDICION");
                csLexer.AddTokenRule("=", "OPERADOR2");
                csLexer.AddTokenRule("{|}", "DELIMITADOR2");

                palabrasReservadas = new List<string>() { "abstract", "as", "async", "await",
                "checked", "const", "continue", "default", "delegate", "base", "break", "case",
                "do", "else", "enum", "event", "explicit", "extern", "false", "finally",
                "fixed", "startfor","endfor" ,"foreach", "goto", "if", "implicit", "in", "interface",
                "internal", "is", "lock", "new", "null", "operator","catch",
                "out", "override", "params", "private", "protected", "public", "readonly",
                "ref", "return", "sealed", "sizeof", "stackalloc", "static",
                "switch", "this", "throw", "true", "try", "typeof", "namespace",
                "unchecked", "unsafe", "virtual", "void", "while", "float", "int", "long", "object",
                "get", "set", "new", "partial", "yield", "add", "remove", "value", "alias", "ascending",
                "descending", "from", "group", "into", "orderby", "select", "where",
                "join", "equals", "using","bool", "byte", "char", "decimal", "double", "dynamic",
                "sbyte", "short", "string", "uint", "ulong", "ushort", "var", "class", "struct","cin","count","lol"};

                csLexer.Compile(RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

                load = true;
                AnalizeCode();
                entrada2.Focus();
                /* lvToken.Columns[0].Width = 250;
                 lvToken.Columns[1].Width = 200;
                 lvToken.Columns[2].Width = 50;
                 lvToken.Columns[3].Width = 50;
                 lvToken.Columns[4].Width = 100;*/

                
            }
        }
       public void modificarTexBox()
        {
            
        }
        private void AnalizeCode()
        {
            lvToken.Rows.Clear();


            int n = 0, e = 0;

            foreach (var tk in csLexer.GetTokens(entrada2.Text))
            {
                if (tk.Name == "ERROR") e++;

                // if (tk.Name == "IDENTIFICADOR")
                if (palabrasReservadas.Contains(tk.Lexema))
                    tk.Name = "RESERVADO";

                lvToken.Rows.Add(tk.Name, tk.Lexema, tk.Index, tk.Linea, tk.Columna);
                n++;
            }

           
            //this.Title = string.Format("Analizador Lexico - {0} tokens {1} errores", n, e);
        }
        public List<string> getIds()
        {
            var lista = new List<string>();

            // MessageBox.Show(lvToken.Rows.Count.ToString());

            for (int i = 0; i < lvToken.Rows.Count - 1; i++)
            {
                if (lvToken.Rows[i].Cells["Nombre"].Value.ToString() == "IDENTIFICADOR")
                {
                    lista.Add(lvToken.Rows[i].Cells["Lexema"].Value.ToString());
                }
            }

            return lista;
        }
        private void Analizar_Click(object sender, EventArgs e)
        {
        }
       
        public void imprimir1(String entrada)
        {
            String[] prints = entrada.Split(';');
            for (int i = 0; i < prints.Length; i++)
            {
                ktf.Kuto imprimir2 = new ktf.Kuto(prints[i]);
                imprimir2 = imprimir2.Extract("print", "");
                imprimir2 = imprimir2.Extract("(",")");

                if (imprimir2.ToString().Contains("+"))
                {
                    String[] prints2 = imprimir2.ToString().Split('+');
                    for (int j = 0; j < prints2.Length; j++)
                    {
                        imprimir(prints2[j]);
                    }
                }
                else
                {
                    imprimir(imprimir2.ToString());
                }
            }

        }

        public void imprimir(String entrada)
        {
            //MessageBox.Show(entrada);
            if (!entrada.Replace(" ","").Equals("")) {
                bool no_se_encontro = false;

                if (entrada.Contains("\""))
                {
                    //imprimir valores de tipo cadena-------------------------------------
                    //MessageBox.Show(codigo.Extract("\"", "\"").ToString());
                    //  MessageBox.Show("ss");
                    ktf.Kuto codigo = new ktf.Kuto(entrada);
                    consola.Text += codigo.Extract("\"", "\"").ToString() + " ";
                    no_se_encontro = true;
                }
                else
                {

                    for (int i = 0; i < vnombre.Count; i++)
                    {
                        if (entrada.Equals(vnombre.ElementAt(i)) && !vvalor.ElementAt(i).Equals(""))
                        {
                            // MessageBox.Show("resultado " + "-"+vvalor.ElementAt(i)+"-");
                            if (vvalor.ElementAt(i).Contains("\""))
                            {
                                ktf.Kuto codigo = new ktf.Kuto(vvalor.ElementAt(i));
                                consola.Text += codigo.Extract("\"", "\"").ToString() + " ";
                                
                            }
                            else {
                                cuadruplos_intermedio cuadriplos = new cuadruplos_intermedio();
                                concatenacion concatenar = new concatenacion();

                                if (vtipo.ElementAt(i).Equals("String")&&vvalor.ElementAt(i).Contains("+")) {

                                    consola.Text +=concatenar.concatenar(vtipo, vnombre, vvalor, vvalor.ElementAt(i)) + " ";
                                    if (consola.Text.Contains("ERROR"))
                                    {
                                        Speedtext("error al concatenar variables");
                                    }
                                }
                                if (vtipo.ElementAt(i).Equals("Double"))
                                {
                                    consola.Text += cuadriplos.realizar_operacion(vnombre, vvalor, vvalor.ElementAt(i).Replace(" ", "")) + " ";
                                    if (consola.Text.Contains("ERROR"))
                                    {
                                        Speedtext("error al relizar operacion");
                                    }
                                }
                                if (vtipo.ElementAt(i).Equals("int"))
                                {
                                    consola.Text += cuadriplos.realizar_operacion(vnombre, vvalor, vvalor.ElementAt(i).Replace(" ", "")) + " ";
                                    if (consola.Text.Contains("ERROR"))
                                    {
                                        Speedtext("error al relizar operacion");
                                    }
                                }
                             
                                //consola.Text += realizar_operacion(vvalor.ElementAt(i).Replace(" ","")) + " ";
                                
                            }
                            no_se_encontro = true;
                        }
                    }

                }
                if (no_se_encontro == false)
                {
                    errorcodigo = false;
                   // MessageBox.Show("no declarada");
                    consola.Text = "VARIABLE NO DECLARADA CON ANTERIORIDAD";
                    Speedtext("VARIABLE NO DECLARADA CON ANTERIORIDAD");
                }
            }
        }
        //--------------------------------------con este metodo se extraen las condicionales if
        public void extraervaloesif(String entrada)
        {
            String v1 = "", v2 = "", comp = "";
            ktf.Kuto valoresif = new ktf.Kuto(entrada);
            ktf.Kuto valoresif2 = new ktf.Kuto(entrada);
            valoresif2 = valoresif2.Extract("(", ")");
            if (valoresif2.ToString().Contains(">"))
            {
                v1 = valoresif2.Extract("", ">").ToString();
                v2 = valoresif2.Extract(">", "").ToString();
                comp = ">";
            }
            if (valoresif2.ToString().Contains("<"))
            {
                v1 = valoresif2.Extract("", "<").ToString();
                v2 = valoresif2.Extract("<", "").ToString();
                comp = "<";
            }
            if (valoresif2.ToString().Contains("=="))
            {
                v1 = valoresif2.Extract("", "==").ToString();
                v2 = valoresif2.Extract("==", "").ToString();
                comp = "==";
            }
            String valor1="", valor2="";
            if (valoresif.ToString().Contains("else")){
                 valor1 = valoresif.Extract("{", "else").ToString();
                //  valoresif = valoresif.Extract("print(", "");
                 valor2 = valoresif.Extract("else", "").ToString();
            }
            else
            {
                valor1 = valoresif.Extract("{", "").ToString();
                //  valoresif = valoresif.Extract("print(", "");
               // valor2 = valoresif.Extract("{", "endif").ToString();
            }
     
          //  MessageBox.Show("valor"+valor1);
            // MessageBox.Show(condicionalif(v1, v2, comp, valor1, valor2));
        //   MessageBox.Show("-----"+v1+ " "+v2+" "+ comp+" -"+"  " +valor1+"  "+ valor2);
             condicionalif(v1, v2, comp, valor1, valor2);
        }
        //-----------------------------------en este metodo se realizan las condicionales if

        public void condicionalif(String v1, String v2, String comp, String salida1, String salida2)
        {
            //  String salida = "";
            bool error = false;
            cuadruplos_intermedio cuadruplos = new cuadruplos_intermedio();
            v1 = v1.Replace(" ", "");
            v2 = v2.Replace(" ", "");
            double intv1 = 0;
            double intv2 = 0;
            try
            {
                intv1 = Convert.ToDouble(v1);
                /*intv2 = Convert.ToDouble(v2);
                encontrarvariables++;*/
            }
          
            catch (Exception e)
            {

                for (int i = 0; i < vvalor.Count; i++)
                {
                    if (v1.Equals(vnombre[i]))
                    {
                        
                        if (vvalor[i].Replace(" ", "").Equals("")|| vvalor[i].Contains("\""))
                        {
                          //  MessageBox.Show("-"+vvalor[i]+"-");
                            errorcodigo = false;
                            error = true;
                            consola.Text = "ERROR EN EL IF";
                            Speedtext("error en la condicional if");
                            break;
                        }
                        //MessageBox.Show(vvalor[i]);
                        try
                        {
                            intv1 = Convert.ToDouble(vvalor[i]);
                        }catch(Exception d)
                        {
                            intv1= Convert.ToDouble(cuadruplos.realizar_operacion(vnombre, vvalor, vvalor[i]));
                           // MessageBox.Show(cuadruplos.realizar_operacion(vnombre, vvalor, vvalor[i]));
                        }
                          
                    }
                }
            }
            try
            {
                intv2 = Convert.ToDouble(v1);
                /*intv2 = Convert.ToDouble(v2);
                encontrarvariables++;*/
            }
            catch (Exception e)
            {

                for (int i = 0; i < vvalor.Count; i++)
                {
                    if (v2.Equals(vnombre[i]))
                    {
                        if (vvalor[i].Replace(" ","").Equals("") || vvalor[i].Contains("\""))
                        {
                           // MessageBox.Show("-" + vvalor[i] + "-");
                            errorcodigo = false;
                            error = true;
                            consola.Text = "ERROR EN EL IF";
                            Speedtext("error en la condicional if");
                            break;
                        }
                       // MessageBox.Show(vvalor[i]);
                        try
                        {
                            intv2 = Convert.ToDouble(vvalor[i]);
                        }
                        catch (Exception d)
                        {
                            intv2 = Convert.ToDouble(cuadruplos.realizar_operacion(vnombre, vvalor, vvalor[i]));
                            // MessageBox.Show(cuadruplos.realizar_operacion(vnombre, vvalor, vvalor[i]));
                        }

                    }
                }
            }
            if (error == false)
            {
                if (comp.Equals(">"))
                {
                    if (intv1 > intv2)
                    {
                        analizarcodigo(salida1);

                    }
                    else
                    {
                        analizarcodigo(salida2);
                        //salida = salida2;
                    }
                }
                if (comp.Equals("<"))
                {
                    if (intv1 < intv2)
                    {
                        analizarcodigo(salida1);
                    }
                    else
                    {
                        analizarcodigo(salida2);
                        //salida = salida2;
                    }
                }
                if (comp.Equals("=="))
                {
                    if (intv1 == intv2)
                    {
                        analizarcodigo(salida1);
                    }
                    else
                    {
                        analizarcodigo(salida2);
                        //salida = salida2;
                    }
                }
            }
        }

        public bool norepetir(String entrada)
        {
            int retorno = 0;

            String[] separar;
            ktf.Kuto scrapper = new ktf.Kuto(entrada);
         //   scrapper = scrapper.Extract("public static void main(String []args){", "}");

            separar = scrapper.ToString().Split(';');
           

            
            for (int i = 0; i < separar.Length; i++)
            {
                if (!separar[i].Replace(" ","").Equals("")) {
                    bool norepetir = true;
               // MessageBox.Show(separar[i]);
                ktf.Kuto scrapper2 = new ktf.Kuto(separar[i]);
                String tipo = scrapper2.Extract("", " ").ToString();
              //  MessageBox.Show(tipo);
                String nombre = scrapper2.Extract(tipo, "=").ToString();
               // MessageBox.Show(nombre);
                String valor = scrapper2.Extract("=", "").ToString();
                tipo = tipo.Replace(" ", "");
                nombre = nombre.Replace(" ", "");
                   
                    if (!valor.Contains("\""))
                    {
                        valor = valor.Replace(" ", "");


                    }
               
                //int k;
                if (nombre.Equals("") && valor.Equals(""))
                {
                    tipo = scrapper2.Extract("", " ").ToString();
                    tipo = tipo.Replace(" ", "");
                    nombre = scrapper2.Extract(tipo, "").ToString();
                    nombre = nombre.Replace(" ", "");

                }
                //para variable=numero;
                if (nombre.Equals("") && !valor.Equals(""))
                {
                    nombre = tipo;
                    tipo = "";
                }

                if (nombre.Equals("if") || nombre.Equals("for") || nombre.Equals("while") || nombre.Equals("case") || nombre.Equals("int")
                    || nombre.Equals("double") || nombre.Equals("float") || nombre.Equals("String"))
                {
                  
                        consola.Text = "NO SE PUEDEN DECLARAR PALABRAS RESERVADAS";
                        Speedtext("no se pueden declarar palabras reservadas");
                        errorcodigo = false;
                    return false;

                }
                for (int j = 0; j < vnombre.Count; j++)
                {
                       // MessageBox.Show(vnombre.ElementAt(j) + " -" + nombre);
                    if (vnombre.ElementAt(j).Equals(nombre) && tipo != "")
                    {
                        consola.Text="NO SE PUEDE DECLARAR LA MISMA VARIABLE";
                        Speedtext("NO SE PUEDE DECLARAR LA MISMA VARIABLE");
                            return false;
                    }
                    if (tipo.Equals("") && nombre.Equals(vnombre.ElementAt(j)))
                    {
                            
                            
                        retorno = 1;
                        try
                        {
                            if (vtipo.ElementAt(j).Equals("String"))
                            {
                                    if (!valor.Contains("\""))
                                    {
                                        consola.Text = "ERROR DE TIPO DE DATO";
                                        Speedtext("ERROR DE TIPO DE DATO");
                                        errorcodigo = false;
                                        break;
                                       
                                    }
                            }
                                if (vtipo.ElementAt(j).Equals("int"))
                            {

                                Convert.ToInt32(valor);
                            }
                            if (vtipo.ElementAt(j).Equals("double"))
                            {
                                Convert.ToDouble(valor);
                            }
                            if (vtipo.ElementAt(j).Equals("boolean"))
                            {
                               // Program.excepcion = "ERROR DE TIPO DE DATO";
                                Convert.ToBoolean(valor);
                            }
                                vvalor[j] = valor;
                                norepetir = false;

                            } catch (Exception e) {
                                consola.Text = "ERROR DE TIPO DE DATO";
                                Speedtext("ERROR DE TIPO DE DATO");
                                return false;
                        }

                        break;
                    }
                    else
                    {
                        if (tipo.Equals("") && retorno != 1)
                        {
                            retorno = 3;
                            consola.Text = "ERROR AL ENCONTRAR VARIABLE";
                            Speedtext("error al encontrar variable");
                            return false; 
                               
                        } } }
                    if (norepetir==true) {
                        vtipo.Add(tipo);
                        vnombre.Add(nombre);
                     //hextra   MessageBox.Show(valor);
                        vvalor.Add(valor);
                       
                    }
            }
            }
            if (retorno == 0 || retorno == 1)
            {
                return true;
               // return true;
            }
            else
            {
                return false;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        public String realizar_operacion(String operacion)
        {   String resultado="";  
            ParseTreeNode arbol = sintatico2.analizar(operacion);
            if (resultado != null)
            {

                // pictureBox1.Image = sintatico.getImage(resultado);
                Recorrido eracion = new Recorrido();
                // Recorrido.reolveroperacion(arbol);
                MessageBox.Show(eracion.reolveroperacion(arbol).ToString());
                resultado= Program.resultado;
            }
            else
            {
                MessageBox.Show("La cadena es incorrecta");
            }
            return resultado;
        }
        private void bunifuFlatButton2_Click(object sender, EventArgs e)
        {
        }
        //--------------------en este metodo se extraen los valores necesarios para realizar el for
        public void extraer_variables_delFor(String entrada)
        {
            /*String entrada = "public class{" +
                "startfor(int i = 0; i > 5 ; i++){ print(\"hola\") }endfor" +
                "}";*/
           // MessageBox.Show("dentro del for");
            ktf.Kuto elfor = new ktf.Kuto(entrada);
            String limite;
            if (entrada.Contains("<"))
            {
                limite = elfor.Extract("<", ";").ToString().Replace(" ", "");
            } else
            {
                limite = elfor.Extract(">", ";").ToString().Replace(" ", "");
            }
            String tamano = elfor.Extract("=", ";").ToString().Replace(" ", "");
            String incremento = elfor.Extract(";", ")").ToString().Replace(" ", "");
          //  MessageBox.Show("dentrofor " + elfor.ToString());
            operacionesfor(limite, tamano,incremento, elfor.Extract("{","").ToString());

        }
        //en ete metodo se realiza el for------------------------------------------------------
        public void operacionesfor(String limite, String tamano, String incremento, String dentrofor)
        {
          
            int limitefor = Convert.ToInt32(limite);
            int tamanofor = Convert.ToInt32(tamano);
            String valorremplazar = "(\" i \")";
            if (dentrofor.Contains("( i )")) ;
            {
                dentrofor = dentrofor.Replace("( i )", valorremplazar);
            }
            if (incremento.Contains("++"))
            {
                for (int i = tamanofor; i < limitefor; i++)
                {
                   
                    analizarcodigo(dentrofor);
                   /* dentrofor = dentrofor.Replace(valorremplazar, "(\" " + Convert.ToString(i) + " \")");
                    valorremplazar = "(\" " + Convert.ToString(i) + " \")";
                    imprimir(dentrofor);*/
                }
            }
            else
            {
                for (int i = tamanofor; i > limitefor; i--)
                {
             //       MessageBox.Show("ss");
                    analizarcodigo(dentrofor);
                    /*
                    dentrofor = dentrofor.Replace(valorremplazar, "(\" " + Convert.ToString(i) + " \")");
                    valorremplazar = "(\" " + Convert.ToString(i) + " \")";
                    imprimir(dentrofor);*/
                }
            }

        }
        public void correr()
        {
            Program.excepcion = "ERROR";
            ParseTreeNode resultado = sintatico.analizar(entrada2.Text);
            if (resultado != null)
            {

                vnombre.Clear();
                vvalor.Clear();
                vtipo.Clear();
                errorcodigo = true;
                ktf.Kuto codigo = new ktf.Kuto(entrada2.Text);
                codigo = codigo.Extract("public static void main(String []args){", "}endmain");
              // MessageBox.Show(codigo.ToString());
                analizarcodigo(codigo.ToString());

                  

                
            }
            else
            {
              
                consola.Text = Program.excepcion;
            }

        }
        public void analizarcodigo(String lineas)
        {
            var codigo = lineas.Split('\n');
            String acumulador = "";
            int acumuladordelineas = 0;
           
            //MessageBox.Show(lineas);
            foreach (var linea in codigo)
            {
                if (errorcodigo == false)
                {
                    break;
                }
                // MessageBox.Show(linea);
                // MessageBox.Show(linea);
                if (linea.Contains(";")&&!linea.Contains("startfor")&&!linea.Contains("print")&& !linea.Contains("ReadLine"))
                {
                    if (norepetir(linea)==false)
                    {
                        errorcodigo = false;
                        break;
                       
                        
                    }
                    
                }
               if (linea.Contains("startif")&&acumuladordelineas ==0|| acumuladordelineas == 1) {
                    acumuladordelineas = 1;
                    acumulador += linea + "\n";
                }
                if (linea.Contains("endif")&&acumuladordelineas == 1)
                {
                    acumuladordelineas = 0;
                    acumulador += linea + "\n";
                   
                    ktf.Kuto relizarcondicional = new ktf.Kuto(acumulador);
                    relizarcondicional = relizarcondicional.Extract("startif", "endif");
                    // MessageBox.Show(relizarcondicional.ToString());
                    acumulador = "";
                    extraervaloesif(relizarcondicional.ToString());
                    
                    
                }
                if (linea.Contains("startfor") && acumuladordelineas == 0 || acumuladordelineas == 2)
                {
                    acumuladordelineas = 2;
                    acumulador += linea+"\n";
                }
                if (linea.Contains("endfor")&& acumuladordelineas==2)
                {
                    acumuladordelineas = 0;
                    acumulador += linea + "\n";

                    ktf.Kuto relizarcondicional = new ktf.Kuto(acumulador);
                    relizarcondicional = relizarcondicional.Extract("startfor", "endfor");
                    acumulador = "";
                    extraer_variables_delFor(relizarcondicional.ToString());
                   

                }

               
                 if (linea.Contains("ReadLine") && acumuladordelineas == 0|| linea.Contains("readline") && acumuladordelineas == 0)
                 {
                     ktf.Kuto linea2 = new ktf.Kuto(linea);
                     String variable = linea2.Extract("", "=").ToString().Replace(" ", "");
                     String entrada = Microsoft.VisualBasic.Interaction.InputBox("INGRESE VALOR DE " + variable, "ENTRADA", "");


                    if( remplazar_variable_valor(variable, entrada)==false)
                    {
                        errorcodigo = false;
                        consola.Text = "ERROR AL INGRESAR VARIABLE";
                        Speedtext("error al ingresar variable");
                        break;
                    }

                 }
               
               // MessageBox.Show("----"+linea);
                if (linea.Contains("print")&& acumuladordelineas == 0 && !linea.Contains("startfor"))
                {
                    //MessageBox.Show("imprimir "+acumuladordelineas);
                    imprimir1(linea);
                }
                if (errorcodigo == false)
                {   
                    break;
                }
            }
        }
        public bool remplazar_variable_valor(String linea, String Valor)
        {
            bool salida = true;
            try
            {
                for (int i = 0; i < vvalor.Count; i++)
            {
                if (linea.Equals(vnombre.ElementAt(i)))
                {
                   
                        if (vtipo.ElementAt(i).Equals("int"))
                        {
                            Convert.ToInt32(Valor);
                        }
                        if (vtipo.ElementAt(i).Equals("Double"))
                        {
                            Convert.ToDouble(Valor);
                        }
                        if (vtipo.ElementAt(i).Equals("Double"))
                        {
                            Convert.ToString(Valor);
                        }
                        vvalor[i] = Valor;
                   
                }
            }
            }
            catch (Exception e)
            {
                salida = false;
            }
            return salida;

        }
       
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        int lineaactual = 0;
        private void entrada_TextChanged(object sender, EventArgs e)
        {
            
            AnalizeCode();
        
            if (lineaactual != entrada2.Lines.Count())
            {
                nlista.Items.Clear();
                for (int i = 1; i < entrada2.Lines.Count()+1; i++)
                {
                    nlista.Items.Add(i);
                }
                }

            lineaactual=entrada2.Lines.Count();
            
           
        }

        private void panel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void lvToken_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void entrada_KeyPress(object sender, KeyPressEventArgs e)
        {

           
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            consola.Text = "   ";
            //cambio1
            correr();
        }

        private void entrada2_TextChanged(object sender, EventArgs e)
        {
            //entrada2.ProcessAllLines();
            entrada2.ProcessAllLines();
            //  MessageBox.Show("alv");

        }
        public void preparar_operacion(String lectura)
        {
            ktf.Kuto nombrevar = new ktf.Kuto(lectura);
            String variable = nombrevar.Extract("int", "=").ToString().Replace(" ", "");
            bool salidaboleana = true;
           
            for (int i = 0; i < vtipo.Count; i++)
            {

                if (vnombre.ElementAt(i).Equals(variable))
                {

                    string operacion = vvalor.ElementAt(i);
                    operacion = operacion.Replace(" ", "");
                    operacion = operacion.Replace("+", "?");
                    operacion = operacion.Replace("-", "?");
                    operacion = operacion.Replace("*", "?");
                    operacion = operacion.Replace("/", "?");

                    String[] operacionseparada = operacion.Split('?');
                    int contavariables = 0;

                    for (int j = 0; j < operacionseparada.Count(); j++)
                    {
                        for (int h = 0; h < vnombre.Count; h++)
                        {
                            if (vnombre.ElementAt(h).Equals(operacionseparada.ElementAt(j)) && !vvalor.ElementAt(h).Equals(""))
                            {
                                vvalor[i] = vvalor[i].Replace(operacionseparada.ElementAt(j), vvalor[h]);
                                contavariables++;

                                break;
                               
                            }
                        }
                    }
                    if (contavariables != operacionseparada.Count())
                    {
                        // MessageBox.Show("error, no  encontraron todas las variables");
                        salidaboleana = false;
                        Program.excepcion = "ERROR EN LA OPERACION";

                    }

                }
                // tablavalores.Rows.Add(vtipo.ElementAt(i), vnombre.ElementAt(i), vvalor.ElementAt(i));
            }
            //return salidaboleana;


        }

        public void lineas()
        {
            
            AnalizeCode();


            if (lineaactual != entrada2.Lines.Count())
            {
                nlista.Items.Clear();
                for (int i = 1; i < entrada2.Lines.Count() + 1; i++)
                {
                    nlista.Items.Add(i);
                }
            }

            lineaactual = entrada2.Lines.Count();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
           
        }

        private void entrada2_KeyPress(object sender, KeyPressEventArgs e)
        {
            lineas();

        }

        private void consola_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void syntaxRichTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void PictureBox9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("dd");
            for (int i = 0; i < vvalor.Count; i++)
            {
                MessageBox.Show(vtipo.ElementAt(i) + " - " + vnombre.ElementAt(i) + " - " + vvalor.ElementAt(i));
            }
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            entrada2.Clear();
        }
    }
}
