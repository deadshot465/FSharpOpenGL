namespace FSharpGL

open System
open OpenTK.Mathematics
open OpenTK.Windowing.Common
open OpenTK.Windowing.Desktop
open OpenTK.Windowing.GraphicsLibraryFramework

module FSharpGL =
    [<EntryPoint>]
    let main argv =
        let gameWindowSettings = GameWindowSettings()
        gameWindowSettings.RenderFrequency <- 60.0
        gameWindowSettings.UpdateFrequency <- 60.0
        gameWindowSettings.IsMultiThreaded <- true
        
        let nativeWindowSettings = NativeWindowSettings()
        nativeWindowSettings.Size <- Vector2i(1024, 768)
        nativeWindowSettings.Profile <- ContextProfile.Core
        nativeWindowSettings.Flags <- ContextFlags.ForwardCompatible
        nativeWindowSettings.Title <- "F# OpenGL"
        nativeWindowSettings.IsFullscreen <- false
        nativeWindowSettings.StartFocused <- true
        nativeWindowSettings.StartVisible <- true
        nativeWindowSettings.API <- ContextAPI.OpenGL
        nativeWindowSettings.AutoLoadBindings <- true
        nativeWindowSettings.NumberOfSamples <- 16
        nativeWindowSettings.APIVersion <- Version(4, 1)
        
        let game = new Game(gameWindowSettings, nativeWindowSettings)
        game.add_KeyDown(fun args ->
            match args.Key with
            | Keys.Escape -> game.Close()
            | _ -> ())
        game.Run()
        0 // return an integer exit code