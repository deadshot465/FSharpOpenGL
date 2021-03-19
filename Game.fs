namespace FSharpGL

open OpenTK.Graphics.OpenGL4
open OpenTK.Windowing.Desktop

type Game(gameWindowSettings: GameWindowSettings, nativeWindowSettings: NativeWindowSettings) =
    inherit GameWindow(gameWindowSettings, nativeWindowSettings)
    let vertices = [-0.5f; -0.5f; 0.0f; 1.0f; 0.0f; 0.0f
                    0.5f; -0.5f; 0.0f; 0.0f; 1.0f; 0.0f;
                    0.0f; 0.5f; 0.0f; 0.0f; 0.0f; 1.0f]
    let vertexShaderCode = @"#version 410 core
    layout (location = 0) in vec3 position;
    layout (location = 1) in vec3 color;
    
    layout (location = 0) out vec3 fragColor;
    
    void main() {
        gl_Position = vec4(position, 1.0);
        fragColor = color;
    }
    "
    
    let fragmentShaderCode = @"#version 410 core
    layout (location = 0) in vec3 fragColor;
    
    out vec4 FragColor;
    
    void main() {
        FragColor = vec4(fragColor, 1.0);
    }
    "
    
    [<DefaultValue>] val mutable vbo: int
    [<DefaultValue>] val mutable vao: int
    [<DefaultValue>] val mutable shaderProgram: int
    [<DefaultValue>] val mutable vertexShader: int
    [<DefaultValue>] val mutable fragmentShader: int

    override this.OnRenderFrame(args) =
        GL.Clear (ClearBufferMask.DepthBufferBit ||| ClearBufferMask.ColorBufferBit)
        
        GL.UseProgram this.shaderProgram
        GL.BindVertexArray this.vao
        GL.DrawArrays (PrimitiveType.Triangles, 0, 3)
        
        this.Context.SwapBuffers()
        base.OnRenderFrame(args)

    override this.OnLoad() =
        GL.Enable (EnableCap.DepthTest ||| EnableCap.Blend ||| EnableCap.Multisample)
        GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f)
        
        this.vbo <- GL.GenBuffer()
        
        this.vertexShader <- GL.CreateShader(ShaderType.VertexShader)
        GL.ShaderSource (this.vertexShader, vertexShaderCode)
        this.fragmentShader <- GL.CreateShader(ShaderType.FragmentShader)
        GL.ShaderSource (this.fragmentShader, fragmentShaderCode)
        
        GL.CompileShader this.vertexShader
        let mutable infoLog = GL.GetShaderInfoLog this.vertexShader
        if infoLog <> "" then eprintfn($"Error when compiling vertex shader: {infoLog}")
        
        GL.CompileShader this.fragmentShader
        infoLog <- GL.GetShaderInfoLog this.fragmentShader
        if infoLog <> "" then eprintfn($"Error when compiling fragment shader: {infoLog}")
        
        this.shaderProgram <- GL.CreateProgram()
        GL.AttachShader (this.shaderProgram, this.vertexShader)
        GL.AttachShader (this.shaderProgram, this.fragmentShader)
        GL.LinkProgram this.shaderProgram
        infoLog <- GL.GetProgramInfoLog (this.shaderProgram)
        if infoLog <> "" then eprintfn($"Error when linking shader program: {infoLog}")
        
        this.vao <- GL.GenVertexArray()
        GL.BindVertexArray this.vao
        GL.BindBuffer (BufferTarget.ArrayBuffer, this.vbo)
        GL.BufferData (BufferTarget.ArrayBuffer, vertices.Length * sizeof<float32>, List.toArray(vertices), BufferUsageHint.StaticDraw)
        GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, 6 * sizeof<float32>, 0)
        GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, 6 * sizeof<float32>, 3 * sizeof<float32>)
        GL.EnableVertexAttribArray 0
        GL.EnableVertexAttribArray 1
        
        base.OnLoad()

    override this.OnUnload() =
        GL.DetachShader (this.shaderProgram, this.vertexShader)
        GL.DetachShader (this.shaderProgram, this.fragmentShader)
        GL.DeleteProgram this.shaderProgram
        GL.DeleteShader this.vertexShader
        GL.DeleteShader this.fragmentShader
        
        GL.BindBuffer (BufferTarget.ArrayBuffer, 0)
        GL.DeleteBuffer (this.vbo)
        base.OnUnload()
