using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Lab2
{
    /// <summary>
    /// Основной класс исполняемой программы, содержащий Main
    /// </summary>
    /// <remarks>
    /// Работа с классами линейных, квадратичных и кубических функций
    /// Входной файл имеет следующий вид: 
    /// Первая строка - целое число - количество функций
    /// Последующие строки - значения для каждой функции на новой строке
    /// Сначала идёт перечисление коэффициентов (2 - линейная, 3 - квадратичная, 4 - кубическая), далее значение x
    /// Разделитель - точка с запятой
    /// Дробные числа могут писаться через точку или запятую
    /// Между двумя соседними числами только один разделитель, перед первым числом и после последнеднего числа разделителей быть не должно
    /// Лишних строк быть не должно
    /// </remarks>
    class Program
    {
        ///<summary>
        ///Точка входа в программу
        ///</summary>
        ///<param name="args">Список аргументов командной строки</param>
        static void Main(string[] args)
        {
            string inputPath = "input.txt";
            string outputPath = "output.txt";
            string inputString;
            List<double>[] equals;
            File.Create(outputPath).Close();
            File.Create("objects.xml").Close();
            try
            {
                Trace.WriteLine("Вызов метода ReadFileIntoString класса FileActions");
                FileActions.ReadFileIntoString(inputPath, out inputString);
                Trace.WriteLine("Вызов метода WriteCoefficientsIntoArray класса FileActions");
                FileActions.WriteCoefficientsIntoArray(inputString, out equals);
                for (int i = 0; i < equals.Length; i++)
                {
                    switch (equals[i].Count)
                    {
                        case 3:
                            Trace.WriteLine("Вызов конструктора метода Line");
                            Line lineFunction = new Line(equals[i][0], equals[i][1]);
                            XmlSerializer serializerLine = new XmlSerializer(typeof(Line));
                            using (StreamWriter sw = new StreamWriter("objects.xml", true, System.Text.Encoding.Default))
                            {
                                serializerLine.Serialize(sw, lineFunction);
                                sw.WriteLine("\n");
                            }
                            Trace.WriteLine("Вызов метода WriteDoubleToTheEndOfFile класса FileActions и метода Solve класса Line");
                            FileActions.WriteDoubleToTheEndOfFile(outputPath, lineFunction.Solve(equals[i][2]));
                            break;
                        case 4:
                            Trace.WriteLine("Вызов конструктора метода Quadratic");
                            Quadratic quadraticFunction = new Quadratic(equals[i][0], equals[i][1], equals[i][2]);
                            XmlSerializer serializerQuadratic = new XmlSerializer(typeof(Quadratic));
                            using (StreamWriter sw = new StreamWriter("objects.xml", true, System.Text.Encoding.Default))
                            {
                                serializerQuadratic.Serialize(sw, quadraticFunction);
                                sw.WriteLine("\n");
                            }
                            Trace.WriteLine("Вызов метода WriteDoubleToTheEndOfFile класса FileActions и метода Solve класса Quadratic");
                            FileActions.WriteDoubleToTheEndOfFile(outputPath, quadraticFunction.Solve(equals[i][3]));
                            break;
                        case 5:
                            Trace.WriteLine("Вызов конструктора метода Cubic");
                            Cubic cubicFunction = new Cubic(equals[i][0], equals[i][1], equals[i][2], equals[i][3]);
                            XmlSerializer serializerCubic = new XmlSerializer(typeof(Cubic));
                            using (StreamWriter sw = new StreamWriter("objects.xml", true, System.Text.Encoding.Default))
                            {
                                serializerCubic.Serialize(sw, cubicFunction);
                                sw.WriteLine("\n");
                            }
                            Trace.WriteLine("Вызов метода WriteDoubleToTheEndOfFile класса FileActions и метода Solve класса Cubic");
                            FileActions.WriteDoubleToTheEndOfFile(outputPath, cubicFunction.Solve(equals[i][4]));
                            break;
                        default:
                            throw new FormatException();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Не найден файл \"input.txt\"");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Не найдена одна из папок в пути к файлу \"input.txt\"");
            }
            catch (PathTooLongException)
            {
                Console.WriteLine("Слишком длинный путь к файлу \"input.txt\"");
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверное содержимое файла \"input.txt\"");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Значение одного из чисел выходит за пределы double");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет доступа для чтения или записи файла");
            }
            finally
            {
                Trace.Flush();
                Console.WriteLine("Нажмите любую клавишу для завершения программы");
                Console.ReadKey();
            };
        }
    }

    /// <summary>
    /// Абстрактный класс, от которого наследуются классы линейной, квадратичной и кубической функций
    /// </summary>
    public abstract class Function
    {
        /// <summary>
        /// Позволяет найти значение функции в точке x
        /// </summary>
        /// <param name="x">Значение точки, в которой ищется значение функции</param>
        /// <returns>Значение функции</returns>
        public abstract double Solve(double x);
    }

    /// <summary>
    /// Класс линейных функций
    /// </summary>
    /// <remarks>
    /// Функция имеет вид y = a*x + b, где x, y - переменные; a, b - коэффициенты
    /// </remarks>
    public class Line : Function
    {
        /// <summary>
        /// Коэффициенты
        /// </summary>
        public double a, b;
        /// <summary>
        /// Конструктор, позволяющий задавать значения коэффициентов при создании объекта класса
        /// </summary>
        /// <param name="a">Значение коэффициента a</param>
        /// <param name="b">Значение коэффициента b</param>
        public Line(double a, double b)
        {
            this.a = a;
            this.b = b;
        }

        public Line() { }

        /// <summary>
        /// Находит значение функции в точке x
        /// </summary>
        /// <param name="x">Значение точки, в которой ищется значение функции</param>
        /// <returns>Значение функции</returns>
        public override double Solve(double x)
        {
            return this.a * x + this.b;
        }
    }

    /// <summary>
    /// Класс квадратичных функций
    /// </summary>
    /// <remarks>
    /// Функция имеет вид y = a*x^2 + b*x + c, где x, y - переменные; a, b, c - коэффициенты
    /// </remarks>
    public class Quadratic : Function
    {
        /// <summary>
        /// Коэффициенты
        /// </summary>
        public double a, b, c;

        /// <summary>
        /// Конструктор, позволяющий задавать значения коэффициентов при создании объекта класса
        /// </summary>
        /// <param name="a">Значение коэффициента a</param>
        /// <param name="b">Значение коэффициента b</param>
        /// <param name="c">Значение коэффициента c</param>
        public Quadratic(double a, double b, double c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Quadratic() { }

        /// <summary>
        /// Находит значение функции в точке x
        /// </summary>
        /// <param name="x">Значение точки, в которой ищется значение функции</param>
        /// <returns>Значение функции</returns>
        public override double Solve(double x)
        {
            return this.a * Math.Pow(x, 2) + this.b * x + this.c;
        }
    }

    /// <summary>
    /// Класс квадратичных функций
    /// </summary>
    /// <remarks>
    /// Функция имеет вид y = a*x^3 + b^2*x + c*x + d, где x, y - переменные; a, b, c, d - коэффициенты
    /// </remarks>
    public class Cubic : Function
    {
        /// <summary>
        /// Коэффициенты
        /// </summary>
        public double a, b, c, d;

        /// <summary>
        /// Конструктор, позволяющий задавать значения коэффициентов при создании объекта класса
        /// </summary>
        /// <param name="a">Значение коэффициента a</param>
        /// <param name="b">Значение коэффициента b</param>
        /// <param name="c">Значение коэффициента c</param>
        /// <param name="d">Значение коэффициента d</param>
        public Cubic(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public Cubic() { }
        /// <summary>
        /// Находит значение функции в точке x
        /// </summary>
        /// <param name="x">Значение точки, в которой ищется значение функции</param>
        /// <returns>Значение функции</returns>
        public override double Solve(double x)
        {
            return this.a * Math.Pow(x, 3) + this.b * Math.Pow(x, 2) + this.c * x + this.d;
        }
    }

    /// <summary>
    /// Класс действий с файлами
    /// </summary>
    public static class FileActions
    {
        /// <summary>
        /// Записывает содержимое файла в строковую переменную
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="outputString">Строка, в которую записывается содержимое файла</param>
        /// <exception cref="FileNotFoundException">Не найден файл</exception>
        /// <exception cref="DirectoryNotFoundException">Не найдена одна из папок в пути к файлу</exception>
        /// <exception cref="PathTooLongException">Cлишком длинный путь к файлу</exception>
        /// <exception cref="UnauthorizedAccessException">Нет доступа для чтения файла</exception>
        /// <exception cref="ArgumentException">Пустой путь к файлу</exception>"
        public static void ReadFileIntoString(string filePath, out string outputString)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                if (new FileInfo(filePath).Length == 0)
                {
                    throw new FormatException();
                }
                outputString = sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Записывает коэффициенты из строки в массив списков
        /// </summary>
        /// <remarks>
        /// Каждый элемент массива - список разделителей для отдельного уравнения
        /// </remarks>
        /// <param name="input">Входная строка</param>
        /// <param name="output">Выходной массив</param>
        /// <exception cref="ArgumentException">Неверное содержимое файла</exception>"
        public static void WriteCoefficientsIntoArray(string input, out List<double>[] output)
        {
            input = input.Replace('.', ',');
            int funcStartPosition = input.IndexOf('\n') + 1;
            int n = Convert.ToInt32(input.Substring(0, funcStartPosition - 1));
            output = new List<double>[n];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = new List<double>();
            }

            string[] functions = input.Substring(funcStartPosition).Split('\n');

            if (functions.Length != n)

            {
                throw new FormatException();
            }

            for (int i = 0; i < functions.Length; i++)
            {
                string[] nums = functions[i].Split(';');
                for (int j = 0; j < nums.Length; j++)
                {
                    output[i].Add(Convert.ToDouble(nums[j]));
                }
            }
        }

        /// <summary>
        /// Записывает число в конец файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="num">Записываемое число</param>
        /// <exception cref="DirectoryNotFoundException">Не найдена одна из папок в пути к файлу</exception>
        /// <exception cref="UnauthorizedAccessException">Нет доступа для записи файла</exception>"
        public static void WriteDoubleToTheEndOfFile(string path, double num)
        {
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(num);
            }
        }
    }
}