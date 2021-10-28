using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using genus.lib;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace genus.app.Graphics
{
	public class EmuWindow : GameWindow
	{
        private int vertexBuffer;
        private int vertexArray;
        private int shaderHandle;
        private int ssboHandle;
        private IntPtr shaderDataPtr;

        private readonly VirtualMachine vm = new();

		public EmuWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
		{
			var rom = File.ReadAllBytes("games/TETRIS");

			vm.Initialize(rom);
			vm.Start();
		}

		protected override void OnLoad()
		{
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Bind vertices
            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Constants.VertexSize, Constants.Vertices, BufferUsageHint.StaticRead);

            // Bind vertex array
            vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(vertexArray);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

			// Initialize SSBO
            var shaderData = CreateGraphicsData();
            var dataSize = Marshal.SizeOf(shaderData);
			shaderDataPtr = Marshal.AllocHGlobal(dataSize);

			ssboHandle = GL.GenBuffer();

			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
			GL.BufferData(BufferTarget.ShaderStorageBuffer, dataSize, shaderDataPtr, BufferUsageHint.DynamicCopy);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);

            // Compile Shaders
            var fragmentSrc = File.ReadAllText("shaders/fragment.glsl");
            var vertexSrc = File.ReadAllText("shaders/vertex.glsl");

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSrc);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSrc);

            GL.CompileShader(fragmentShader);
            GL.CompileShader(vertexShader);

            PrintShaderLog(vertexShader);
            PrintShaderLog(fragmentShader);

            shaderHandle = GL.CreateProgram();
            GL.AttachShader(shaderHandle, vertexShader);
			GL.AttachShader(shaderHandle, fragmentShader);

            GL.LinkProgram(shaderHandle);

            // Cleanup shader code
            GL.DetachShader(shaderHandle, vertexShader);
            GL.DetachShader(shaderHandle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Use shader (duh)
            GL.UseProgram(shaderHandle);

			base.OnLoad();
		}

        private shader_data CreateGraphicsData()
        {
			var gfxData = new int[vm.GfxBuffer.Length];
			// System.Buffer.BlockCopy(vm.GfxBuffer, 0, gfxData, 0, vm.GfxBuffer.Length);

			for (var i = 0; i < vm.GfxBuffer.Length; i++)
            {
				gfxData[i] = vm.GfxBuffer[i];
            }

            shader_data shaderData;
            shaderData.pixels = gfxData;

            return shaderData;
        }

        private void PrintShaderLog(int shader)
        {
            var info = GL.GetShaderInfoLog(shader);

			if (!string.IsNullOrWhiteSpace(info))
			{
				Console.WriteLine(info);
                Debugger.Break();
			}
        }

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
            GL.UseProgram(shaderHandle);

            GL.BindVertexArray(vertexArray);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

			Context.SwapBuffers();
			base.OnRenderFrame(args);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			base.OnUpdateFrame(args);

            var shaderData = CreateGraphicsData();
			GL.BindBuffer(BufferTarget.ShaderStorageBuffer, ssboHandle);
            var gpuPtr = GL.MapBuffer(BufferTarget.ShaderStorageBuffer, BufferAccess.ReadWrite);
			Marshal.StructureToPtr(shaderData, gpuPtr, false);
            GL.UnmapBuffer(BufferTarget.ShaderStorageBuffer);

			var escapeDown = KeyboardState.IsKeyDown(Keys.Escape);
			if (escapeDown)
			{
				Close();
			}
		}

		protected override void OnResize(ResizeEventArgs e)
		{
            GL.Viewport(0, 0, 640, 320);
			base.OnResize(e);
		}

		protected override void OnUnload()
		{
            // Unbind buffers
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Clear buffers
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteVertexArray(vertexArray);

            GL.DeleteProgram(shaderHandle);

			base.OnUnload();
		}
	}
}