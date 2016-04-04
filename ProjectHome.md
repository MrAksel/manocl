ManOCL is an acronym for Managed OpenCL, it is a .NET 2.0 friendly wrapper that simplifies host OpenCL initialization, execution and profiling.

Currently ManOCL is in it's Alpha stage of development.

Current release was tested under Windows XP/Vista/7 and Mac OS X 10.6.7 (Mono). It is also expected to work under Linux (Mono)

## Don't hesitate to contact me ##


---

**New Version 0.10:**
  * Added OpenGL interop support (Mac currently)
  * Fixed memory leak
  * Tested under 100% load, 0 problems found during 336 hours of production load.

**Description:**
Current version is tested in one of my projects under heavy load. Previous version had a leak, currently no leaks found, memory usage is stable even after two weeks under heavy load (Mac). I confirm that OpenGL interop also works well (also Mac)

**Current known limitations:**
Due to the fact that it is not possible to initialize shared memory for OpenCL from the host, using DeviceLocalMemory in kernel argument list currently is not allowed, though you can always declare shared memory inside kernel code...

---


Here's an example of real-life host part:

```
using System;
using System.Net;
using System.Collections.Generic;
using System.IO;

using ManOCL;
using ManOCL.IO;

namespace Kernel
{
    public class ApplicationClass
    {
        static Random rand = new Random();

        static Single[] RandomArray(Int32 length)
        {
            Single[] result = new Single[length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (Single)rand.NextDouble();
            }

            return result;
        }

        static void Main(string[] args)
        {
            DeviceGlobalMemory output = new Byte[4096];

            DeviceGlobalMemory indeces = RandomArray(102400);
            DeviceGlobalMemory ops = new Byte[3072];
            DeviceGlobalMemory data = RandomArray(1048576);

            Console.Write("Creating kernel...");

            Kernel kernel = Kernel.Create("Kernel", File.ReadAllText("Test.c"), data, indeces, ops, output);

            Console.Write("Executing kernel...");

            Event e = kernel.Execute(256, 256);

            kernel.CommandQueue.Finish();

            Console.WriteLine("done, operation took {0}", Profiler.DurationSeconds(e));

            UnmanagedReader reader = new UnmanagedReader(new DeviceBufferStream(output));

            for (int i = 0; i < 256; i++)
            {
                if (i % 4 == 0) Console.WriteLine();
                if (i % 16 == 0) Console.WriteLine();

                Console.Write("{0}\t", reader.Read<Single>());
            }
        }
    }
}
```

Here is a OpenGL interop initialization part, straight from my project
```
namespace TestNamespace
{
	public class TestClass
	{
		public static void Run()
		{
			Random random = new Random();
					
			Int32 bpp = 32;
			Int32 width = 640;
			Int32 height = 480;
		
			var format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
        
			Int32 depth = filenames.Length;
		
			byte[] image3D = new byte[depth * width * height * bpp];
					
			Int32 bytesCopied = 0;
		
			foreach (var filename in filenames)
			{
				// Fill image3d
			}
					
			INativeWindow wnd = new OpenTK.NativeWindow(800, 600, "OpenGL", GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);
			IGraphicsContext ctx = new GraphicsContext(GraphicsMode.Default, wnd.WindowInfo);
        
			wnd.Visible = true;
			wnd.Resize += delegate { InitViewport(wnd, ctx); };
			wnd.KeyPress += delegate(object sender, OpenTK.KeyPressEventArgs e) {
				if (e.KeyChar == 'q')
				{
					wnd.Close();
				}
				else if (e.KeyChar == '=' || e.KeyChar == '+')
				{
					Size size = wnd.Size;
					Point location = wnd.Location;
				
					wnd.Location = new Point(location.X - 16, location.Y);
					wnd.Size = new Size(size.Width + 32, size.Height + 32);
				}
				else if (e.KeyChar == '-')
				{
					Size size = wnd.Size;
					Point location = wnd.Location;
				
					wnd.Location = new Point(location.X + 16, location.Y + 44);
					wnd.Size = new Size(size.Width - 32, size.Height - 32);
				}
			};
		
			ctx.MakeCurrent(wnd.WindowInfo);
			ctx.LoadAll();
		
			InitGL(wnd, ctx);
					
			uint fbo = CreateFBO();
					
			uint t2Mona = CreateTexture2D(GL.GL_TEXTURE0, width, height);
			uint t2Best = CreateTexture2D(GL.GL_TEXTURE0, width, height);
			uint t2Current = CreateTexture2D(GL.GL_TEXTURE0, width, height);
			uint t3Bulb = CreateTexture3D(GL.GL_TEXTURE1, width, height, depth);
		
			LoadTextureBitmap(t2Mona, File.ReadAllText("Kernels/KernelA.mona"));
		
			GCHandle image3DHandle = GCHandle.Alloc(image3D, GCHandleType.Pinned);
			{
				GL.glBindTexture(GL.GL_TEXTURE_3D, t3Bulb);
				{
					GL.glTexImage3D(GL.GL_TEXTURE_3D, 0, GL.GL_RGBA, width, height, depth, 0, GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, image3DHandle.AddrOfPinnedObject());
				}
				GL.glBindTexture(GL.GL_TEXTURE_3D, 0);
			}
			image3DHandle.Free();
					
			InitViewport(wnd, ctx);
					
			ManOCL.Context.Default = ManOCL.Context.ShareWithCGL(CGL.GetShareGroup(CGL.GetCurrentContext()));
			ManOCL.CommandQueue.Default = ManOCL.CommandQueue.Create(CommandQueueProperties.ProfilingEnable);
					
			DeviceSampler sampler = DeviceSampler.Create();
		
			DeviceImage a = DeviceImage.CreateFromTexture2D(t2Mona, DeviceBufferAccess.Read);
			DeviceImage b = DeviceImage.CreateFromTexture2D(t2Current, DeviceBufferAccess.Read);

			Single[,] resultsArray = new Single[width, height];
		
			DeviceGlobalMemory results = DeviceGlobalMemory.CreateFromArray(resultsArray);
		
			Kernel estimator = ManOCL.Kernel.Create
			(
				 "Estimate", 
				@"typedef float image_buffer[{0}][{1}];
				__kernel void Estimate(sampler_t smp, read_only image2d_t a, read_only image2d_t b, __global image_buffer *img)
				{
				    float4 colorA = read_imagef(a, smp, (int2)(get_global_id(0), get_global_id(1))),
						   colorB = read_imagef(b, smp, (int2)(get_global_id(0), get_global_id(1)));

					float x = colorA.x > 0.04045f ? pow((colorA.x + 0.055f) / 1.055f, 2.2f) : (colorA.x / 12.92f);
					float y = colorA.y > 0.04045f ? pow((colorA.y + 0.055f) / 1.055f, 2.2f) : (colorA.y / 12.92f);
					float z = colorA.z > 0.04045f ? pow((colorA.z + 0.055f) / 1.055f, 2.2f) : (colorA.z / 12.92f);
				
					colorA.x = x * 0.4124f + y * 0.3576f + z * 0.1805f;
					colorA.y = x * 0.2126f + y * 0.7152f + z * 0.0722f;
					colorA.z = x * 0.0193f + y * 0.1192f + z * 0.9505f;
				
					x = colorB.x > 0.04045 ? pow((colorB.x + 0.055f) / 1.055f, 2.2f) : (colorB.x / 12.92f);
					y = colorB.y > 0.04045 ? pow((colorB.y + 0.055f) / 1.055f, 2.2f) : (colorB.y / 12.92f);
					z = colorB.z > 0.04045 ? pow((colorB.z + 0.055f) / 1.055f, 2.2f) : (colorB.z / 12.92f);
				
					colorB.x = x * 0.4124f + y * 0.3576f + z * 0.1805f;
					colorB.y = x * 0.2126f + y * 0.7152f + z * 0.0722f;
					colorB.z = x * 0.0193f + y * 0.1192f + z * 0.9505f;

					(*img)[get_global_id(0)][get_global_id(1)] =
					((colorA.x - colorB.x) * (colorA.x - colorB.x)) +
					((colorA.y - colorB.y) * (colorA.y - colorB.y)) +
					((colorA.z - colorB.z) * (colorA.z - colorB.z)) +
					((colorA.w - colorB.w) * (colorA.w - colorB.w));
				}"
			 	.Replace("{0}", resultsArray.GetLength(1).ToString())
			 	.Replace("{1}", resultsArray.GetLength(0).ToString()),
				sampler, a, b, results
			);
		
			Single[] reductedResultsArray = new Single[0x80];
		
			DeviceGlobalMemory reductedResults = DeviceGlobalMemory.CreateFromArray(reductedResultsArray);
		
			Kernel reductor = Kernel.Create
			(
				"Reduct",
				@"__kernel void Reduct(__global float *source, __global float *dest)
				{
					float result = 0.0f;

					int index = get_global_id(0);

					while (index < {0})
					{
						result += source[index];

						index += get_global_size(0);
					}

					barrier(CLK_LOCAL_MEM_FENCE);

					dest[get_global_id(0)] = result;
				}"
			 	.Replace("{0}", reductedResultsArray.Length.ToString()),
		 		results, reductedResults
		 	);
		
			// Irrelevant code further ...
		}

		private static void InitViewport(INativeWindow wnd, IGraphicsContext ctx)
		{
			GL.glViewport(0, 0, wnd.Width, wnd.Height);
			GL.glMatrixMode(GL.GL_PROJECTION);
			GL.glLoadIdentity();
			GL.glMatrixMode(GL.GL_MODELVIEW);
			GL.glLoadIdentity();
			
			Double aspect = 1;
			
			if (wnd.Height > 0)
			{
		 		aspect = wnd.Width / (double)wnd.Height;
			}

			Double square = 3;
			
			Double realWidth = square * aspect;
			
			GL.glOrtho(-realWidth * 0.5, realWidth * 0.5, -square * 0.5, square * 0.5, -1, 1);
				
			ctx.Update(wnd.WindowInfo);
		}
		
		private static void InitGL(INativeWindow wnd, IGraphicsContext ctx)
		{
			GL.glShadeModel(GL.GL_SMOOTH);
			
			GL.glActiveTexture(GL.GL_TEXTURE0);
			GL.glEnable(GL.GL_TEXTURE_2D);
			
			GL.glActiveTexture(GL.GL_TEXTURE1);
			GL.glEnable(GL.GL_TEXTURE_3D);

			GL.glActiveTexture(GL.GL_TEXTURE0);
			
			GL.glEnable(GL.GL_BLEND);
			GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
			
			GL.glDisable(GL.GL_DEPTH_TEST);
			
			GL.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
			GL.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
		}
		
		static uint CreateTexture2D(uint slot, Int32 width, Int32 height)
		{
			GL.glActiveTexture(slot);

			uint texture;
			
			GL.glGenTextures(1, out texture);
			GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
			GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, width, height, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			GL.glTexParameterf(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);
			GL.glTexParameterf(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
			GL.glTexParameterf(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP);
			GL.glTexParameterf(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP);
			GL.glBindTexture(GL.GL_TEXTURE_2D, 0);
			
			return texture;
		}

		static uint CreateTexture3D(uint slot, Int32 width, Int32 height, Int32 depth)
		{
			GL.glActiveTexture(slot);
			
			uint texture;
			
			GL.glGenTextures(1, out texture);
			GL.glBindTexture(GL.GL_TEXTURE_3D, texture);
			GL.glTexImage3D(GL.GL_TEXTURE_3D, 0, GL.GL_RGBA, width, height, depth, 0, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, IntPtr.Zero);
			GL.glTexParameterf(GL.GL_TEXTURE_3D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);
			GL.glTexParameterf(GL.GL_TEXTURE_3D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);
			GL.glTexParameterf(GL.GL_TEXTURE_3D, GL.GL_TEXTURE_WRAP_S, GL.GL_CLAMP);
			GL.glTexParameterf(GL.GL_TEXTURE_3D, GL.GL_TEXTURE_WRAP_T, GL.GL_CLAMP);
			GL.glTexParameterf(GL.GL_TEXTURE_3D, GL.GL_TEXTURE_WRAP_R, GL.GL_CLAMP);
			GL.glBindTexture(GL.GL_TEXTURE_3D, 0);
			
			return texture;
		}

		static uint CreateFBO()
		{
			uint fbo;
			
			GL.glGenFramebuffers(1, out fbo);
						
			return fbo;
		}
		static Double[] RandomArray(Int32 length, Random random)
		{
			Double[] result = new Double[length];
			
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = random.NextDouble();
			}
			
			return result;
		}

				
		static void RenderTexture(uint texture, float x, float y, float z)
		{
			GL.glPushMatrix();
			GL.glPushAttrib(GL.GL_TEXTURE_BIT | GL.GL_CURRENT_BIT);
			{
				GL.glTranslatef(x, y, z);
				
				GL.glColor4f(1, 1, 1, 1);
				
				GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
				GL.glBegin(GL.GL_QUADS);
				{
					GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex2f(0, 0);
					GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex2f(0, 1);
					GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex2f(1, 1);
					GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex2f(1, 0);
				}
				GL.glEnd();
				GL.glBindTexture(GL.GL_TEXTURE_2D, 0);
			}
			GL.glPopAttrib();
			GL.glPopMatrix();
		}
		
		static void LoadTextureBitmap(uint texture, String path)
		{
			Bitmap bmp = new Bitmap(path);
			
			BitmapData bmpBits = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			{
				GL.glActiveTexture(GL.GL_TEXTURE0);
				GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
				{
					GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, bmpBits.Width, bmpBits.Height, 0, GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, bmpBits.Scan0);
				}
				GL.glBindTexture(GL.GL_TEXTURE_2D, 0);
			}
			bmp.UnlockBits(bmpBits);
		}
		
		static void RenderScene(uint slot, uint currentTexture, uint bestTexture, uint monaTexture)
		{
			GL.glActiveTexture(slot);

			GL.glClear(GL.GL_COLOR_BUFFER_BIT);
			
			GL.glPushMatrix();
			GL.glPushAttrib(GL.GL_TEXTURE_BIT | GL.GL_CURRENT_BIT);
			{
				GL.glTranslatef(-0.5f, -0.5f, 0);
				
				RenderTexture(bestTexture, 1f, 0, 0);
				RenderTexture(monaTexture, 0f, 0, 0);				
				RenderTexture(currentTexture, -1f, 0, 0);				
			}
			GL.glPopAttrib();
			GL.glPopMatrix();
		}
	}
}
```