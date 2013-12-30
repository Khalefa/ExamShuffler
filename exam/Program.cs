using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace exam
{
    class Program
    {

        abstract class shuffable
        {
            public static Random r;
            public int getshuffle(XmlNode qs, int l)
            {
                if (qs == null) return 0;
                XmlAttribute attr = qs.Attributes["shuffle"];
                if (attr == null)
                    return 0;
                if (attr.Value == "all")
                {
                    return l;
                }
                else
                {
                    return Int32.Parse(attr.Value);
                }

            }
            public string getText(XmlNode nn)
            {
                XmlNodeList l = nn.ChildNodes;
                string t1 = "";
                foreach (XmlNode n in l)
                {
                    if (n.NodeType == XmlNodeType.Text)
                    {
                        string t = n.Value;
                        t = t.Replace("<", "$<$");
                        t = t.Replace(">", "$>$");
                        t = t.Replace("$$", "");
                        t1 = t1 + t;
                    }
                    if ((n.NodeType == XmlNodeType.Element && n.Name == "text"))
                        t1 = t1 + n.InnerText.Replace("\\end{lstlisting}", "\\end{lstlisting}\\vspace{-0.4in}"); ;

                }
                return t1;
            }
            protected int shuffle;
            public abstract void Shuffle();
            public shuffable[] Shuffle(shuffable[] list)
            {
                List<shuffable> l = new List<shuffable>();
                for (int i = 0; i < shuffle; i++)
                {
                    l.Add(list[i]);
                }
                List<shuffable> rr = Randomize(l.ToArray());
                for (int i = shuffle; i < list.Length; i++)
                {
                    rr.Add(list[i]);
                }

                return rr.ToArray();
            }

            List<shuffable> Randomize(shuffable[] list)
            {
                List<shuffable> randomized = new List<shuffable>();
                List<shuffable> original = new List<shuffable>(list);

                while (original.Count > 0)
                {
                    int index = r.Next(original.Count);
                    randomized.Add(original[index]);
                    original.RemoveAt(index);
                }

                return randomized;
            }

        }
        class option : shuffable
        {
            public option(XmlNode n)
            {
                text = n.InnerText;
            }
            public string text;
            public override string ToString()
            {
                string t = "";
                t = text.Replace("\n", "\\\\");
                //t = t.Replace(">", "$>$");
                //t = t.Replace("<", "$<$");
                return "\\choice " + t + "\n";
            }
            public override void Shuffle()
            {
                throw new NotImplementedException();
            }
        }

        class part : shuffable
        {
            public part(XmlNode x)
            {
                text = getText(x);

                options = new List<option>();
                XmlElement qs = x["options"];
                if(qs!=null)
                foreach (XmlNode n in qs.ChildNodes)
                {
                    option o = new option(n);
                    options.Add(o);
                }
                shuffle = getshuffle(qs, options.Count);
            }
            public string text;
            public List<option> options;
            public override string ToString()
            {
                string s = "\\part \n";
                string t="";
                s =  s+ text + " \n";
           
                
                foreach (option o in options)
                    t = t + o.ToString();
                if(t.Contains("\\\\") || t.Length >100)
                    s = s + "\n \\begin{choices}" + "\n" +t+ "\n \\end{choices}" + "\n";
                else
                    if(options.Count>0)
                    s = s + "\n \\begin{oneparchoices}" + "\n" + t + "\n \\end{oneparchoices}" + "\n";
                return s;
            }
            public  string ToString1()
            {
                string s = "";
                string t = "";
                s = text + " \n";


                foreach (option o in options)
                    t = t + o.ToString();
                if (t.Contains("\\\\") || t.Length > 100)
                    s = s + "\n \\begin{choices}" + "\n" + t + "\n \\end{choices}" + "\n";
                else
                    if (options.Count > 0)
                        s = s + "\n \\begin{oneparchoices}" + "\n" + t + "\n \\end{oneparchoices}" + "\n";
                return s;
            }
            public override void Shuffle()
            {
                shuffable[] qar = options.ToArray();
                foreach (option q in qar)
                {
                    //q.Shuffle();

                }
                shuffable[] sqar = Shuffle(qar);
                options.Clear();
                foreach (option q in sqar)
                {

                    options.Add(q);
                }
            }
        }
        class question : shuffable
        {
            public question(XmlNode x)
            {
                text = getText(x);
                parts = new List<part>();
                XmlElement qs = x["parts"];
                if (qs == null) return;
                foreach (XmlNode n in qs.ChildNodes)
                {
                    part p = new part(n);
                    parts.Add(p);
                }
                shuffle = getshuffle((XmlNode)(qs), parts.Count);
            }
            public string text;
            public List<part> parts;
            public  string ToString1()
            {
                string s = "\\question";
                if (parts.Count == 1)
                    s = s + "\n" + text + parts[0].ToString();
                else
                {
                    //string s = "";// "\\begin{question}" + "\n";
                    s = text;
                    //if (parts.Count > 1)
                    {
                        //s = s + "\\begin{parts}" + "\n";
                        foreach (part q in parts)
                            s = s + "\\question " + "\n" + q.ToString();

                        //s = s + "\\end{parts}" + "\n";
                    }
                    /*else
                    {
                        %s = s + "%" + parts[0].ToString();
                    }
                    //s = s + "\\end{question}" + "\n";*/
                }
                return s;
            }
            
            public override string ToString()
            {
                string s = "\\begin{question}";
                    s = s+text;
                    if (parts.Count > 1)
                    {
                        s = s + "\\begin{parts}" + "\n";
                        foreach (part q in parts)
                            s = s +q.ToString();

                        s = s + "\\end{parts}" + "\n";
                    }
                    else
                        if (parts.Count ==1)
                    {
                        s = s + "%" + parts[0].ToString();
                    }
                        
                    s = s + "\\end{question}" + "\n";
                
                return s;
            }
            public override void Shuffle()
            {
                shuffable[] qar = parts.ToArray();
                foreach (part q in qar)
                {
                    q.Shuffle();

                }
                shuffable[] sqar = Shuffle(qar);
                parts.Clear();
                foreach (part q in sqar)
                {

                    parts.Add((part)q);
                }
            }
        }
        class Exam : shuffable
        {

            public Exam(XmlNode x, int version)
            {
                start = System.IO.File.ReadAllText(@"C:\Users\khalefa\SkyDrive\Teaching\Programming II\private\Exam\header.tex");
                start = start.Replace("{version}", version.ToString());
                XmlElement qs = x["questions"];

                questions = new List<question>();
                foreach (XmlNode n in qs.ChildNodes)
                {
                    question q = new question(n);
                    questions.Add(q);
                }

                shuffle = getshuffle(qs, questions.Count);
            }


            public string start;
            public List<question> questions;
            public override string ToString()
            {
                string s = start + "\n";
                s = s + "\\begin{questions}" + "\n";
                foreach (question q in questions)
                    s = s + q.ToString();
                s = s + "\\end{questions}" + "\n";
                s = s + "\\end{document}" + "\n";
                return s;
            }

            public override void Shuffle()
            {
                shuffable[] qar = questions.ToArray();
                foreach (question q in qar)
                {
                    q.Shuffle();

                }
                shuffable[] sqar = Shuffle(qar);
                questions.Clear();
                foreach (question q in sqar)
                {

                    questions.Add((question)q);
                }
            }
        }
        static string header = "\\documentclass[11pt,a4paper]{exam} \n"
        + "\\usepackage[final]{pdfpages}" + "\n" +
            "\\begin{document}" + "\n";

        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:/Users/khalefa/Downloads/exam.xml");
            string dirname = @"c:\Users\khalefa\Downloads\exam\";
            string batch = dirname + "compile.bat";
            string combined = dirname + "c.tex";
            string com_str = header;
            string batch_str = "";

            for (int i = 0; i < 120; i++)
            {
                int version = i;
                shuffable.r = new Random(version);
                string filename = dirname + "a_" + version.ToString() + ".tex";

                Exam e = new Exam(doc["exam"], version);

               e.Shuffle();
                e.ToString();
                System.IO.File.WriteAllText(filename, e.ToString());
                batch_str = batch_str + "pdflatex a_" + version.ToString() + "\n";
                com_str = com_str + " \\includepdf[pages=-]{a_" + version.ToString() + ".pdf}" + "\n";
            }

            batch_str = batch_str + "pdflatex " + "c.tex";
            com_str = com_str + "\\end{document}";
            System.IO.File.WriteAllText(batch, batch_str);
            System.IO.File.WriteAllText(combined, com_str);
        }

    }
}
