using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;
using System.Linq;
using System.Threading;
using OpenTK;

using Vector4 = System.Numerics.Vector4;

namespace VectorPerf
{
    struct MyVec
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        public static MyVec Normalize(MyVec v)
        {
            var s = 1.0f / MathF.Sqrt (v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            return new MyVec
            {
                X = v.X * s,
                Y = v.Y * s,
                Z = v.Z * s,
                W = v.W * s,
            };
        }
        public static MyVec operator *(MyVec v, float s)
        {
            return new MyVec
            {
                X = v.X * s,
                Y = v.Y * s,
                Z = v.Z * s,
                W = v.W * s,
            };
        }
        public static MyVec operator +(MyVec v, MyVec w)
        {
            return new MyVec
            {
                X = v.X + w.X,
                Y = v.Y + w.Y,
                Z = v.Z + w.Z,
                W = v.W + w.W,
            };
        }
    }

    struct MyVecd
    {
        public double X;
        public double Y;
        public static MyVecd operator *(MyVecd v, float s)
        {
            return new MyVecd
            {
                X = v.X * s,
                Y = v.Y * s,
            };
        }
        public static MyVecd operator +(MyVecd v, MyVecd w)
        {
            return new MyVecd
            {
                X = v.X + w.X,
                Y = v.Y + w.Y,
            };
        }
    }

    public static class PerfRunner
    {
        const int ArraySize = 1 << 14;

        static readonly MyVecd[] mvdX = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(42);
            return new MyVecd { X = rand.NextDouble(), Y = rand.NextDouble() };
        }).ToArray();
        static readonly MyVecd[] mvdX2 = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(142);
            return new MyVecd { X = rand.NextDouble(), Y = rand.NextDouble() };
        }).ToArray();
        static readonly MyVecd[] mvdY = new MyVecd[ArraySize];

        static readonly float[] aX = ((Func<float[]>)(() =>
        {
            var rand = new Random(42);
            var a = new float[4 * ArraySize];
            for (var i = 0; i < 4 * ArraySize; i++)
            {
                a[i] = (float)rand.NextDouble();
            }
            return a;
        }))();
        static readonly float[] aX2 = ((Func<float[]>)(() =>
        {
            var rand = new Random(142);
            var a = new float[4 * ArraySize];
            for (var i = 0; i < 4 * ArraySize; i++)
            {
                a[i] = (float)rand.NextDouble();
            }
            return a;
        }))();
        static readonly float[] aY = new float[ArraySize * 4];

        static readonly MyVec[] mvX = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(42);
            return new MyVec { X = (float)rand.NextDouble(), Y = (float)rand.NextDouble(), Z = (float)rand.NextDouble(), W = (float)rand.NextDouble() };
        }).ToArray();
        static readonly MyVec[] mvX2 = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(142);
            return new MyVec { X = (float)rand.NextDouble(), Y = (float)rand.NextDouble(), Z = (float)rand.NextDouble(), W = (float)rand.NextDouble() };
        }).ToArray();
        static readonly MyVec[] mvY = new MyVec[ArraySize];

        static readonly Vector4[] v4X = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(42);
            return new Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
        }).ToArray();
        static readonly Vector4[] v4X2 = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(142);
            return new Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
        }).ToArray();
        static readonly Vector4[] v4Y = new Vector4[ArraySize];

        static readonly OpenTK.Vector4[] tv4X = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(42);
            return new OpenTK.Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
        }).ToArray();
        static readonly OpenTK.Vector4[] tv4X2 = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(142);
            return new OpenTK.Vector4((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
        }).ToArray();
        static readonly OpenTK.Vector4[] tv4Y = new OpenTK.Vector4[ArraySize];

        static readonly Vector2d[] v2dX = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(42);
            return new Vector2d(rand.NextDouble(), rand.NextDouble());
        }).ToArray();
        static readonly Vector2d[] v2dX2 = Enumerable.Range(0, ArraySize).Select(_ =>
        {
            var rand = new Random(142);
            return new Vector2d(rand.NextDouble(), rand.NextDouble());
        }).ToArray();
        static readonly Vector2d[] v2dY = new Vector2d[ArraySize];

        static readonly Vector<double>[] vtdX = Enumerable.Range(0, ArraySize).Select(_ => {
            var rand = new Random(42);
            var vals = new double[Vector<double>.Count];
            vals[0] = rand.NextDouble();
            vals[1] = rand.NextDouble();
            return new Vector<double>(vals);
        }).ToArray();
        static readonly Vector<double>[] vtdX2 = Enumerable.Range(0, ArraySize).Select(_ => {
            var rand = new Random(142);
            var vals = new double[Vector<double>.Count];
            vals[0] = rand.NextDouble();
            vals[1] = rand.NextDouble();
            return new Vector<double>(vals);
        }).ToArray();
        static readonly Vector<double>[] vtdY = new Vector<double>[ArraySize];

        static readonly Vector<float>[] vtX = Enumerable.Range(0, ArraySize).Select(_ => {
            var rand = new Random(42);
            var vals = new float[Vector<float>.Count];
            vals[0] = (float)rand.NextDouble();
            vals[1] = (float)rand.NextDouble();
            vals[2] = (float)rand.NextDouble();
            vals[3] = (float)rand.NextDouble();
            return new Vector<float>(vals);
        }).ToArray();
        static readonly Vector<float>[] vtX2 = Enumerable.Range(0, ArraySize).Select(_ => {
            var rand = new Random(142);
            var vals = new float[Vector<float>.Count];
            vals[0] = (float)rand.NextDouble();
            vals[1] = (float)rand.NextDouble();
            vals[2] = (float)rand.NextDouble();
            vals[3] = (float)rand.NextDouble();
            return new Vector<float>(vals);
        }).ToArray();
        static readonly Vector<float>[] vtY = new Vector<float>[ArraySize];

        static void MyVecScale()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                mvY[i] = mvX[i] * 42.0f;
            }
        }

        static void TKVector4Scale()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                tv4Y[i] = tv4X[i] * 42.0f;
            }
        }

        static void Vector4Scale()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                v4Y[i] = v4X[i] * 42.0f;
            }
        }

        static void VectorTScale()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                vtY[i] = vtX[i] * 42.0f;
            }
        }

        static void ArrayScale()
        {
            for (var i = 0; i < ArraySize * 4;)
            {
                aY[i] = aX[i] * 42.0f;
                i++;
                aY[i] = aX[i] * 42.0f;
                i++;
                aY[i] = aX[i] * 42.0f;
                i++;
                aY[i] = aX[i] * 42.0f;
                i++;
            }
        }

        static void MyVecAdd()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                mvY[i] = mvX[i] + mvX2[i];
            }
        }

        static void Vector4Add()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                v4Y[i] = v4X[i] + v4X2[i];
            }
        }

        static void TKVector4Add()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                tv4Y[i] = tv4X[i] + tv4X2[i];
            }
        }

        static void VectorTAdd()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                vtY[i] = vtX[i] + vtX2[i];
            }
        }

        static void ArrayAdd()
        {
            for (var i = 0; i < ArraySize * 4;)
            {
                aY[i] = aX[i] + aX2[i];
                i++;
                aY[i] = aX[i] + aX2[i];
                i++;
                aY[i] = aX[i] + aX2[i];
                i++;
                aY[i] = aX[i] + aX2[i];
                i++;
            }
        }

        static void MyVecNorm()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                mvY[i] = MyVec.Normalize(mvX[i]);
            }
        }

        static void TKVector4Norm()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                tv4Y[i] = OpenTK.Vector4.Normalize(tv4X[i]);
            }
        }

        static void Vector4Norm()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                v4Y[i] = Vector4.Normalize (v4X[i]);
            }
        }

        static void VectorTNorm()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                var v = vtX[i];
                var x = 1.0f / MathF.Sqrt (v[0] * v[0] + v[1] * v[1] + v[2] * v[2] + v[3] * v[3]);
                vtY[i] = v * x;
            }
        }

        static void MyVec2dAdd()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                mvdY[i] = mvdX[i] + mvdX2[i];
            }
        }

        static void TKVector2dAdd()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                v2dY[i] = v2dX[i] + v2dX2[i];
            }
        }

        static void VectorTdAdd()
        {
            for (var i = 0; i < ArraySize; i++)
            {
                vtdY[i] = vtdX[i] + vtdX2[i];
            }
        }

        static TimeSpan TimeTest(Action action, int loops)
        {
            var stopwatch = new Stopwatch();

            // Warmup
            for (var i = 0; i < loops / 10; i++)
                action();

            // Time
            stopwatch.Start();
            for (var i = 0; i < loops; i++)
                action();
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }

        static void RunTest(string name, Action action)
        {
            var loops = 1 << 12;
            var time = TimeTest(action, loops);
            while (time.TotalSeconds < 3 && loops < (1 << 24))
            {
                loops *= 2;
                time = TimeTest(action, loops);
            }

            var secondsPerIteration = time.TotalSeconds / loops;

            Console.WriteLine($"{name} = {secondsPerIteration:0.000000000} ({loops} loops)");
            Thread.Sleep(1000); // Try to avoid iOS's wrath
        }

        public static void Run()
        {
            Console.WriteLine($"Vector<T> has {Vector<float>.Count} floats, or {Vector<double>.Count} doubles, or {Vector<int>.Count} ints");
            Console.WriteLine("==");
            RunTest("    MyVecScale", MyVecScale);
            RunTest("TKVector4Scale", TKVector4Scale);
            RunTest("  Vector4Scale", Vector4Scale);
            RunTest("  VectorTScale", VectorTScale);
            RunTest("    ArrayScale", ArrayScale);
            Console.WriteLine("--");
            RunTest("     MyVecNorm", MyVecNorm);
            RunTest(" TKVector4Norm", TKVector4Norm);
            RunTest("   Vector4Norm", Vector4Norm);
            RunTest("   VectorTNorm", VectorTNorm);
            Console.WriteLine("--");
            RunTest("      MyVecAdd", MyVecAdd);
            RunTest("  TKVector4Add", TKVector4Add);
            RunTest("    Vector4Add", Vector4Add);
            RunTest("    VectorTAdd", VectorTAdd);
            RunTest("      ArrayAdd", ArrayAdd);
            Console.WriteLine("--");
            RunTest("    MyVec2dAdd", MyVec2dAdd);
            RunTest(" TKVector2dAdd", TKVector2dAdd);
            RunTest("   VectorTdAdd", VectorTdAdd);
            Console.WriteLine("==");
        }

        public static Task RunAsync() => Task.Run(Run);
    }
}
