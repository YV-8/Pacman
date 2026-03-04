# Guia Readme
## Proyecto PacMan — Programación III
### Tecnologías: `C#` · `.NET 9` · `Avalonia UI 11` · `CommunityToolkit.Mvvm`

> **Objetivo:** Dominio total de la arquitectura para responder cualquier pregunta técnica y realizar modificaciones de código en vivo durante la defensa.

---

## Tabla de Contenidos

1. [Anatomía y Flujo del Sistema (Big Picture)](#1-anatomía-y-flujo-del-sistema-big-picture)
   - [Arquitectura MVVM](#11-arquitectura-mvvm-en-tu-proyecto)
   - [Flujo completo de información](#12-flujo-completo-de-información-en-el-sistema)
   - [Data Bindings de Avalonia](#13-data-bindings-de-avalonia-ui--cómo-funcionan-exactamente)
   - [Patrones de diseño](#14-patrones-de-diseño-identificados-y-cómo-justificarlos)
2. [Análisis Crítico de los Módulos Core](#2-análisis-crítico-de-los-módulos-core)
   - [IA de los Fantasmas](#21-inteligencia-artificial-de-los-fantasmas)
   - [Sistema de Audio](#22-sistema-de-audio--defensa-crítica)
   - [Gestión de Estado y Colisiones](#23-gestión-de-estado-colisiones-puntuación-y-condiciones-de-juego)
   - [Archivos, Assets e I/O](#24-archivos-assets-e-io)
3. [Mapa de Navegación del Código](#3-mapa-de-navegación-del-código--árbol-de-decisión)
   - [Árbol de decisión](#32-árbol-de-decisión-para-modificaciones-en-vivo)
   - [Mapa de archivos críticos](#33-mapa-de-archivos-críticos)
   - [Preguntas trampa](#34-preguntas-trampa-frecuentes-y-respuestas-directas)

---

# 1. Anatomía y Flujo del Sistema

## 1.1 Arquitectura MVVM en tu proyecto

Tu proyecto implementa MVVM de forma estricta usando **CommunityToolkit.Mvvm 8.2.1** sobre **.NET 9** con **Avalonia UI 11**. La separación de capas es:

| Capa | Responsabilidad | Clases principales |
|---|---|---|
| **MODEL** | Lógica de negocio pura, sin referencias a UI | `Entity`, `Ghost`, `Pacman`, `GameEngine`, `ScoreService`, `SoundManager` |
| **VIEW** | UI declarativa en AXAML. No tiene lógica | `MainWindow.axaml`, `GameView.axaml`, `PacmanView.axaml`, `ScoreBoardView.axaml` |
| **VIEWMODEL** | Puente entre Model y View. Expone propiedades observables y comandos | `GameViewModel`, `PacmanViewModel`, `GhostViewModel`, `ScoreBoardViewModel` |

---

## 1.2 Flujo completo de información en el sistema

El flujo arranca en `Program.cs → BuildAvaloniaApp()` → `App.axaml.cs.OnFrameworkInitializationCompleted()` que instancia:

```
MainWindow { DataContext = new MainWindowViewModel() }
```

A partir de ahí, el flujo de navegación es:

```
[Usuario click "Jugar"]
        │
        ▼
Button Command="{Binding NavigateCommand}" CommandParameter="GoGame"
        │
        ▼
MainWindowViewModel.NavigateCommand → Navigation.ChangePage("GoGame")
        │
        ▼
ManagePageChange.ChangePage() → CurrentPage = new GameViewModel(this)
        │
        ▼
ContentControl en MainWindow.axaml tiene {Binding Navigation.CurrentPage}
El binding detecta el cambio → ViewLocator resuelve el tipo
        │
        ▼
ViewLocator: "GameViewModel" → reemplaza "ViewModel" por "View" → "GameView"
Instancia GameView via Activator.CreateInstance(type)
        │
        ▼
GameView.axaml se renderiza con DataContext = instancia de GameViewModel
```

---

## 1.3 Data Bindings de Avalonia UI — Cómo funcionan exactamente

### Compiled Bindings (activados por defecto)

El `.csproj` tiene:
```xml
<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
```

Esto significa que cada binding se verifica en **tiempo de compilación**. Para que funcionen, cada View declara su DataContext con:
```xml
x:DataType="vm:GameViewModel"
```

> **Excepción:** El `ItemsControl` en `PacmanView.axaml` usa `x:CompileBindings="False"` porque el template trabaja con `GameObject` de forma dinámica.

---

### Cómo se notifican los cambios a la UI (`INotifyPropertyChanged`)

#### Con `[ObservableProperty]` (la mayoría de tus propiedades)

Anotas un campo privado y el **source generator** de CommunityToolkit genera automáticamente la propiedad pública con su `OnPropertyChanged()`. Ejemplo en `Entity.cs`:

```csharp
[ObservableProperty]
private double _canvasLeft;
// El generador produce internamente:
// public double CanvasLeft { get => _canvasLeft; set => SetProperty(ref _canvasLeft, value); }
```

La View, al estar enlazada con `{Binding CanvasLeft}`, recibe la notificación y se actualiza automáticamente.

#### Con `OnPropertyChanged()` manual

En `PacmanViewModel`, `Row` y `Col` tienen setter manual porque necesitan ejecutar lógica adicional:

```csharp
public int Row
{
    get => _row;
    set { _row = value; PacmanModel.UpdateCanvasPosition(); OnPropertyChanged(nameof(Row)); }
}
```

---


## 1.4 Patrones de Diseño identificados y cómo justificarlos

| Patrón | Dónde está | Justificación académica |
|---|---|---|
| **MVVM** | Arquitectura principal | Separa UI de lógica, facilita testing, mantenibilidad. La View no sabe nada del Model. |
| **Strategy** | Comportamiento de fantasmas | `GhostBehaviorBase` = estrategia abstracta. `BlinkyBehavior`, `PinkyBehavior`, `InkyBehavior`, `ClydeBehavior` = estrategias concretas. `Ghost.AssignDirection()` selecciona en runtime. |
| **Template Method** | `GhostBehaviorBase` | `GetBestDirectionToTarget()` define el algoritmo esqueleto. Las subclases solo calculan `targetRow/targetCol`. |
| **Observer (eventos .NET)** | `GameEngine` | Expone `PacmanDied`, `LevelComplete`, `OnEnergizerEaten`, `GhostEaten`. `GameViewModel.Animation.cs` se suscribe con `+=`. Es el patrón Observer nativo de C#. |
| **Flyweight** | `SpriteManager` | Cachea `Bitmap` ya cargados en `_spriteCache`. Múltiples entidades comparten la misma instancia en memoria. |
| **Factory Method** | `EngineManager` | `CreateCellFromChar()` recibe un `char` del mapa y retorna la `Entity` correcta (`Board`, `Pellet`, `Ghost`, `Pacman`). |
| **DTO** | `InteractionResultObject` | Transfiere resultado de una colisión entre capas sin exponer entidades completas. |
| **Service / Repository** | `ScoreService` | Clase abstracta estática que actúa como repositorio de persistencia JSON. |

---

# 2. Análisis Crítico de los Módulos Core

## 2.1 Inteligencia Artificial de los Fantasmas

### Jerarquía del sistema de IA

```
GhostBehaviorBase (abstract)
├── GetBestDirectionToTarget()   ← algoritmo de navegación A→B (compartido)
├── GetScatterDirection()        ← lógica de esquinas (compartida)
└── DistanceDelta()              ← deltas de movimiento por dirección

    ├── BlinkyBehavior    → DecideNextDirection()  [target = Pacman exacto]
    ├── PinkyBehavior     → DecideNextDirection()  [target = 4 celdas adelante]
    ├── InkyBehavior      → DecideNextDirection()  [target = cálculo vectorial con Blinky]
    └── ClydeBehavior     → DecideNextDirection()  [target = Pacman O esquina según distancia]

Ghost.AssignDirection()  ← despacha a la estrategia correcta según ghost.Type + ghost.State
GhostViewModel.MoveGhosts()  ← loop principal que orquesta todo por tick
```

---

### Comportamiento específico de cada fantasma

| Fantasma | Tipo | Lógica exacta | Archivo |
|---|---|---|---|
| **Blinky** | `REDGHOST` | Target = posición EXACTA de Pacman. El más agresivo. `MoveInterval=2` (más rápido). | `BlinkyBehavior.cs` |
| **Pinky** | `PINKGHOST` | Target = **4 celdas adelante** de Pacman según su dirección. Intenta cortarle el paso. | `PinkyBehavior.cs` |
| **Inky** | `CYANGHOST` | Target = punto calculado con Blinky como pivote. Toma 2 celdas adelante de Pacman y **duplica el vector Blinky→pivote**. El más complejo. | `InkyBehavior.cs` |
| **Clyde** | `ORANGEGHOST` | Si distancia a Pacman **< 6 celdas**: huye a esquina `(30, 1)`. Si está lejos: persigue como Blinky. | `ClydeBehavior.cs` |

---

### Modos de comportamiento global

Cada fantasma tiene **dos estados ortogonales**:

- **`GhostState`**: `INHOUSE`, `NORMAL`, `FRIGHTENED`, `DEAD`
- **`GhostHunterMode`**: `Scatter`, `Chase`

El ciclo Scatter/Chase se gestiona en `GhostViewModel.UpdateHunterMode()`:

```csharp
// ScatterDuration = 45 ticks, ChaseDuration = 80 ticks
var cycleLife = _modeCycleTimer % (ScatterDuration + ChaseDuration);
GhostHunterMode newMode = cycleLife < ScatterDuration ? GhostHunterMode.Scatter : GhostHunterMode.Chase;
```

Las esquinas de Scatter por fantasma están en `GhostBehaviorBase.GetScatterCorner()`:

```csharp
EntityType.REDGHOST    → (0, 27)   // esquina superior derecha
EntityType.PINKGHOST   → (0, 0)    // esquina superior izquierda
EntityType.CYANGHOST   → (30, 27)  // esquina inferior derecha
EntityType.ORANGEGHOST → (30, 0)   // esquina inferior izquierda
```

El modo **FRIGHTENED** (activado por energizador) dura **8000ms** y usa movimiento aleatorio:

```csharp
// GhostViewModel.StartFrightenedMode()
_timerFrighten = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8000) };
```

---

### Dónde tocar para modificaciones en vivo

| Modificación pedida | Archivo exacto | Método / línea |
|---|---|---|
| Cambiar velocidad de un fantasma | `Models/Entities/Ghost.cs` | `GetMoveInterval()` — reducir número = más rápido |
| Cambiar objetivo de persecución | `Models/Ghosts/[Nombre]Behavior.cs` | `DecideNextDirection()` — modifica `targetRow`, `targetCol` |
| Cambiar duración del modo asustado | `ViewModels/Ghost/GhostViewModel.cs` | `StartFrightenedMode()` — valor `8000` ms |
| Cambiar esquina de Scatter | `Models/Ghosts/GhostBehaviorBase.cs` | `GetScatterCorner()` |
| Cambiar tiempo de salida de la casa | `Models/Entities/Ghost.cs` | `SetupInitialState()` — `ExitDelayTicks` por tipo |

---

## 2.2 Sistema de Audio — Defensa Crítica

### Arquitectura del `SoundManager`

```
PlaySound(nameSong, isLooping)
        │
        ├─ StopSound()  ← detiene proceso anterior
        │
        ├─ Task.Run()   ← hilo del ThreadPool (no bloquea UI)
        │       │
        │       ├─ AssetLoader.Open(uri)           ← carga WAV desde Avalonia Resources
        │       ├─ CopyTo(FileStream)               ← escribe en archivo temporal en GetTempPath()
        │       └─ PlayWithSystemPlayer()           ← lanza Process("paplay", tempFile)
        │                   │
        │                   └─ Process.Exited += () => { if (_shouldLoop) PlayWithSystemPlayer(); }
        │
        └─ StopSound() → _currentProcess.Kill()    ← mata el proceso del SO
```

Los archivos WAV están embebidos como **Avalonia Resources** en `Assets/Media/` y se acceden con URI:
```
avares://PacmanSolution/Assets/Media/{nameSong}.wav
```

---

### Uso the tecnologia

> **Respuesta estructurada para la defensa:**
>
> *"El sistema de audio fue el módulo de mayor complejidad técnica del proyecto por dos razones:*
>
> **1) Latencia y compatibilidad:** Avalonia UI 11 no incluye una API nativa de audio. Evalué las tres librerías referenciadas en el `.csproj` (`ManagedBass`, `NAudio`, `OpenTK.OpenAL`), pero cada una introdujo problemas: `ManagedBass` requiere `libbass.so` instalado en el sistema, `NAudio` tiene soporte limitado en Linux, y `OpenTK.OpenAL` requería configurar un contexto OpenAL complejo. La solución con `paplay` (reproductor nativo de PulseAudio en Linux) elimina dependencias de terceros y delega al reproductor del SO.
>
> **2) Concurrencia:** El audio debe ejecutarse sin bloquear el game loop. `Task.Run()` despacha la operación al ThreadPool de .NET, manteniendo el `DispatcherTimer` del juego en el hilo de UI sin interrupciones.
>
> *Asistí con IA para investigar y validar este patrón específico porque el time-to-market era crítico y la documentación de integración Avalonia + audio es escasa."*

---

### Limitaciones conocidas y cómo defenderlas

La implementación actual reproduce **un sonido a la vez** (`StopSound()` antes de `PlaySound()`). Si el profesor lo señala:

> *"La decisión fue intencional para la v1. La arquitectura ya está preparada para superposición: cada instancia de `SoundManager` maneja un `Process` independiente. En el código actual ya existen dos instancias paralelas: `_soundManager` en `PacmanViewModel` (sonido de muerte) y `soundManager` en `GameViewModel` (música de fondo)."*

---

### Dónde tocar para modificaciones en vivo

| Modificación pedida | Archivo | Qué cambiar |
|---|---|---|
| Silenciar la música de fondo | `ViewModels/PacmanGame/GameViewModel.cs` | `ToggleAudio()` — comenta `soundManager.PlaySound()` |
| Silenciar el sonido de muerte | `ViewModels/Pacman/PacmanViewModel.cs` | `DeathAudioCommand()` — comenta `_soundManager.PlaySound()` |
| Cambiar un archivo de audio | Cualquier llamada a `PlaySound()` | Cambia el string del nombre. El archivo debe existir en `Assets/Media/` |
| Agregar sonido para un evento | El ViewModel que maneja ese evento | Instancia `SoundManager` y llama `PlaySound("nombre", false)` |

---

## 2.3 Gestión de Estado, Colisiones, Puntuación y Condiciones de Juego

### El archivo que controla todas las reglas del juego

> **`Models/Game/GameEngine.cs`** — La clase más importante del sistema. Es el cerebro del juego.

### Constantes de puntuación (todas en `GameEngine.cs`)

```csharp
private const int DotPoints       = 10;
private const int EnergizerPoints = 50;
private const int cherryPoints    = 100;
private const int GhostPoints     = 200;  // base, se multiplica en GhostViewModel

// En GhostViewModel.cs:
private static readonly int[] GhostPoints = { 200, 400, 800, 1600 }; // combo por ronda
```

---

### Eventos del juego (patrón Observer en `GameEngine.cs`)

```csharp
public event Action?     PacmanDied;         // → GameViewModel pausa y lanza animación de muerte
public event Action?     OnEnergizerEaten;   // → GhostViewModel.SetFrightened()
public event Action?     LevelComplete;      // → GameViewModel muestra overlay "WINNER!"
public event Action<int>? GhostEaten;        // → Score.Score += points
```

Suscripciones en `GameViewModel.Animation.cs`:
```csharp
_engine.OnEnergizerEaten += () => { Ghosts.SetFrightened(); Ghosts.StartFrightenedMode(); };
_engine.GhostEaten       += points => Score.Score += points;
_engine.LevelComplete    += () => { _gameTimer.Stop(); ShowWinOverlay = true; };
_engine.PacmanDied       += () => { PauseAllTimers(); Pacman.DeathAnimation(); };
```

---

### Flujo completo de colisión Pacman-Punto

```
PacmanViewModel.GetMovePacman()
        │
        ├─ PacmanModel.MovePacman(Row, Col)   ← calcula nextRow, nextCol
        ├─ engine.CanMoveTo(targetEntity)     ← false si WALL o DOOR → no mueve
        │
        ├─ engine.InteractionObjects(target)  ← retorna InteractionResultObject
        │       └─ ChooseEffectPellet()
        │               ├─ dot       → PointsEarned = 10
        │               ├─ energizer → PointsEarned = 50 + OnEnergizerEaten.Invoke()
        │               └─ EatenPellets++ → si >= TotalPellets → LevelComplete.Invoke()
        │
        ├─ Score.Amount(result.PointsEarned)  ← actualiza ScoreBoardViewModel
        ├─ targetEntity.IsActive = false      ← dispara PropertyChanged
        │       └─ GameBoardSyncService.OnEntityChanged() → HideDot() → remueve visual del Canvas
        │
        └─ UpdatePacmanPosition(nextRow, nextCol)
```

---

### Flujo completo de colisión Pacman-Fantasma

```csharp
// GameEngine.CollisionsToPacman()
if (ghost.Row == pacman.Row && ghost.Col == pacman.Col)
{
    if (ghost.State is DEAD || INHOUSE)  → return 0; // sin efecto

    if (ghost.State == FRIGHTENED)
    {
        GhostEaten?.Invoke(GhostPoints);   // puntos al Score
        ghost.RespawnGhost(ghost);          // State = DEAD, cuenta regresiva
        return -1;
    }

    if (ghost.State == NORMAL)
    {
        pacman.RespawnPacman();             // vuelve al spawn
        ghost.RespawnAllGhost(_board);      // todos los fantasmas a sus casas
        PacmanDied?.Invoke();               // → DeathAnimation → decrementar vidas
    }
}
```

---

### Dónde tocar para modificaciones en vivo

| Modificación pedida | Archivo | Método / constante |
|---|---|---|
| Cambiar puntos de dot | `Models/Game/GameEngine.cs` | `const int DotPoints` |
| Cambiar puntos de energizador | `Models/Game/GameEngine.cs` | `const int EnergizerPoints` |
| Cambiar puntos por fantasma | `ViewModels/Ghost/GhostViewModel.cs` | array `GhostPoints` |
| Cambiar condición de victoria | `Models/Game/GameEngine.cs` | `ChooseEffectPellet()`, condición `EatenPellets >= TotalPellets` |
| Cambiar número de vidas | `ViewModels/PacmanGame/GameViewModel.cs` | `_countLivePacman = 3` |
| Alterar reglas de colisión | `Models/Game/GameEngine.cs` | `CollisionsToPacman()` |

---

## 2.4 Archivos, Assets e I/O

### Carga de assets gráficos — `SpriteManager`

```csharp
// Patrón Flyweight en LoadSprite():
private readonly Dictionary<string, Bitmap> _spriteCache = new();

private Bitmap? LoadSprite(string imagenPath)
{
    if (_spriteCache.TryGetValue(imagenPath, out var sprite)) return sprite; // cache hit
    Uri uri = new Uri($"avares://PacmanSolution/Assets/Imagen/SpritesPacman/{imagenPath}");
    var bitmap = new Bitmap(AssetLoader.Open(uri));
    _spriteCache[imagenPath] = bitmap; // almacena para reutilización
    return bitmap;
}
```

`GetSpriteSection()` retorna un `CroppedBitmap` — una **vista recortada** del bitmap completo sin duplicar memoria.

---

### Cómo se calcula el frame correcto del spritesheet

**Para Pacman** (`PacmanViewModel.GetPacmanSprite()`):
```csharp
// Spritesheet: PacmanViews.png — frames de 32x32 px
var rect = new PixelRect(_animationFrame * 32, _currentSpriteRow * 32, 32, 32);
// _animationFrame: 0 o 1 (boca abierta/cerrada)
// _currentSpriteRow: 0=RIGHT, 1=LEFT, 2=UP, 3=DOWN
// Frame de muerte: row=2, col va del 0 al 11 (DeathTotalFrames = 12)
```

**Para Fantasmas** (`GhostViewModel.GetGhostSprite()`):
```csharp
// Spritesheet: GhostViews.png — frames de 16x16 px
int col = GetDirectionBaseCol(ghost.Direction) + _globalAnimationFrame;
// GetDirectionBaseCol: Right=0, Left=2, Up=4, Down=6
// Cada dirección ocupa 2 columnas (frame 0 y 1)
// Fila: Rojo=0, Rosa=1, Cyan=2, Naranja=3
// FRIGHTENED: col=8 o 9, fila=0
// DEAD (ojos): col=9 o 10, fila=1
```

---

### El tablero — `Board.cs`

El mapa está hardcodeado en `Board.cs` como `readonly string[] Layout`. Cada carácter es interpretado por `EngineManager.CreateCellFromChar()`:

| Carácter | Entidad creada |
|---|---|
| `'W'` | `Board` con `EntityType.WALL` |
| `'-'` | `Board` con `EntityType.DOOR` (puerta de la casa de fantasmas) |
| `'.'` | `Pellet` (dot, `isEnergizer=false`, 4×4 px) |
| `'o'` | `Pellet` (energizador, `isEnergizer=true`, 8×8 px) |
| `'P'` | `Pacman` (posición de spawn inicial) |
| `'G'` | `Ghost`. `_ghostCount` determina el tipo: 0=Rojo, 1=Rosa, 2=Cyan, 3=Naranja |
| `'E'` o `' '` | `Board` con `EntityType.EMPTY` |

---

### Persistencia de puntuación — `ScoreService`

```csharp
// Ruta del archivo en el sistema:
private static readonly string FilePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "PacmanSolution",
    "scores.json"   // → %AppData%/PacmanSolution/scores.json en Windows
);

// Carga: System.Text.Json deserializa List<Score>
public static List<Score> LoadScores() { ... }

// Guardado: crea el directorio si no existe, serializa con WriteIndented=true
public static void SaveScore(List<Score> scores) { ... }
```

`ScoreBoardViewModel.SaveData()` valida: exactamente 3 caracteres, solo letras (`char.IsLetter`). Luego: carga → agrega → guarda → recarga `HighScores`.

---

### `GameBoardSyncService` — El bridge entre lógica y visual

Este servicio mantiene un `Dictionary<Entity, GameObject>` que sincroniza el estado lógico con el Canvas:

```
Entity (lógica) → PropertyChanged → OnEntityChanged() → GameObject (visual en Canvas)

IsActive = false        →   HideDot()          → _visualObjects.Remove(visual)
Row o Col cambia        →   visual.X = col * CellWidth;  visual.Y = row * CellHeight
CurrentDisplaySprite    →   visual.Sprite = entity.CurrentDisplaySprite
```

---

### Si el profesor pide agregar un nuevo tipo de bloque

```
1. EntityType.cs          → agregar el nuevo valor al enum
2. Board.cs               → agregar el carácter al string[] Layout
3. EngineManager.cs       → agregar case en CreateCellFromChar() que retorne la nueva Entity
4. GameEngine.cs          → definir en CanMoveTo() si es transitable
                          → definir en InteractionObjects() si tiene efecto al tocarlo
5. GameBoardSyncService.cs → agregar case en CreateVisualForEntity() si necesita sprite/visual
```

---

# 3. Mapa de Navegación del Código — Árbol de Decisión

## 3.1 Regla mnemotécnica principal

```
¿El cambio es VISUAL, de REGLA/LÓGICA, o de COMPORTAMIENTO de un actor?
         │                   │                          │
      → View             → Engine/VM               → Behavior/VM
```

---


## 3.2 Mapa de archivos críticos

| Archivo | Qué controla (lo más importante) |
|---|---|
| `GameEngine.cs` | **CEREBRO**: colisiones, puntos, eventos del juego, condición de victoria/derrota |
| `GameViewModel.cs` + `.Animation.cs` | **COORDINADOR**: instancia todo, gestiona timers, suscribe eventos, overlays Win/GameOver |
| `GhostViewModel.cs` | **IA DE FANTASMAS**: loop de movimiento, sprites, modo asustado, puntos por fantasma |
| `PacmanViewModel.cs` | **MOVIMIENTO PACMAN**: input → movimiento → animación → muerte → respawn |
| `[Nombre]Behavior.cs` | **ALGORITMO** de persecución específico de cada fantasma |
| `Board.cs` | **MAPA**: `string[] Layout` con la disposición del tablero |
| `GameConfig.cs` | **CONSTANTES GLOBALES**: tamaños de celda, offsets del canvas |
| `ScoreService.cs` | **I/O DE PUNTUACIÓN**: lectura/escritura JSON en AppData |
| `SoundManager.cs` | **AUDIO**: carga WAV de assets, ejecuta proceso del sistema, loop |
| `SpriteManager.cs` | **SPRITES**: carga, caché y recorte de bitmaps desde assets |
| `GameBoardSyncService.cs` | **BRIDGE**: sincroniza entidades lógicas con objetos visuales del Canvas |
| `ManagePageChange.cs` | **NAVEGACIÓN**: factory de ViewModels, controla `CurrentPage` observable |
| `ViewLocator.cs` | **MVVM BRIDGE**: resuelve `ViewModel` → `View` por nombre via reflexión |

---
*Programación III · Proyecto PacMan · .NET 9 + Avalonia UI 11 · CommunityToolkit.Mvvm 8.2*
