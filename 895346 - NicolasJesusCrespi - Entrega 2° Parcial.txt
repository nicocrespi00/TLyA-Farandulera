using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<Post> posts = new List<Post>();
    static int nextId = 1;
    static string rolActual = "";

    static void Main(string[] args)
    {
        while (true)
        {
            MenuPrincipal();
        }
    }

    // ===================== MENÚ PRINCIPAL =====================

    static void MenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════╗");
        Console.WriteLine("║     SISTEMA LA FARANDULERA       ║");
        Console.WriteLine("╚══════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("Seleccione su rol:");
        Console.WriteLine("  1. Martín / La Faraona");
        Console.WriteLine("  2. Samantha");
        Console.WriteLine("  3. El Manager");
        Console.WriteLine("  4. La Tortuga Legendaria");
        Console.WriteLine("  0. Salir");
        Console.WriteLine();
        Console.Write("Opción: ");

        switch (Console.ReadLine())
        {
            case "1": MenuMartin(); break;
            case "2": MenuSamantha(); break;
            case "3": MenuManager(); break;
            case "4": MenuTortuga(); break;
            case "0": Environment.Exit(0); break;
            default:
                Console.WriteLine("Opción inválida.");
                Pausar();
                break;
        }
    }

    static void Encabezado()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════╗");
        Console.WriteLine("║     SISTEMA LA FARANDULERA       ║");
        Console.WriteLine($"║  ROL: {rolActual,-28}║");
        Console.WriteLine("╚══════════════════════════════════╝");
        Console.WriteLine();
    }

    // ===================== MENÚ MARTÍN =====================

    static void MenuMartin()
    {
        rolActual = "Martín / La Faraona";
        bool salir = false;
        while (!salir)
        {
            Encabezado();
            Console.WriteLine("1. Dar de alta un post");
            Console.WriteLine("2. Editar un post");
            Console.WriteLine("3. Publicar un post");
            Console.WriteLine("4. Archivar un post");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Opción: ");

            switch (Console.ReadLine())
            {
                case "1": AltaPost(null); break;
                case "2": EditarPost(null); break;
                case "3": PublicarPost(); break;
                case "4": ArchivarPost(); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); Pausar(); break;
            }
        }
    }

    // ===================== MENÚ SAMANTHA =====================

    static void MenuSamantha()
    {
        rolActual = "Samantha";
        List<Categoria> permiso = new List<Categoria> { Categoria.Tatianas, Categoria.Nereas };
        bool salir = false;
        while (!salir)
        {
            Encabezado();
            Console.WriteLine("1. Dar de alta un post (Tatianas / Nereas)");
            Console.WriteLine("2. Editar un post (Tatianas / Nereas)");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Opción: ");

            switch (Console.ReadLine())
            {
                case "1": AltaPost(permiso); break;
                case "2": EditarPost(permiso); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); Pausar(); break;
            }
        }
    }

    // ===================== MENÚ MANAGER =====================

    static void MenuManager()
    {
        rolActual = "El Manager";
        bool salir = false;
        while (!salir)
        {
            Encabezado();
            Console.WriteLine("1. Eliminar un post");
            Console.WriteLine("2. Emitir informes");
            Console.WriteLine("0. Volver al menú principal");
            Console.WriteLine();
            Console.Write("Opción: ");

            switch (Console.ReadLine())
            {
                case "1": EliminarPost(); break;
                case "2": EmitirInformes(); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); Pausar(); break;
            }
        }
    }

    // ===================== MENÚ TORTUGA =====================

    static void MenuTortuga()
    {
        rolActual = "La Tortuga Legendaria";
        Encabezado();
        ScrollearPosts();
    }

    // ===================== OPERACIONES =====================

    static void AltaPost(List<Categoria> categoriasPermitidas)
    {
        Encabezado();
        Console.WriteLine("-- NUEVO POST --\n");

        string titulo;
        do
        {
            Console.Write("Título: ");
            titulo = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(titulo))
                Console.WriteLine("El título no puede estar vacío. Ingreselo nuevamente.");
        } while (string.IsNullOrWhiteSpace(titulo));

        List<Categoria> opciones = categoriasPermitidas ?? Enum.GetValues(typeof(Categoria)).Cast<Categoria>().ToList();
        int catIdx;
        do
        {
            Console.WriteLine("Categoría:");
            for (int i = 0; i < opciones.Count; i++)
                Console.WriteLine($"  {i + 1}. {NombreCategoria(opciones[i])}");
            Console.Write("Opción: ");
        } while (!int.TryParse(Console.ReadLine(), out catIdx) || catIdx < 1 || catIdx > opciones.Count);
        Categoria categoria = opciones[catIdx - 1];

        string contenido;
        do
        {
            Console.Write("Contenido: ");
            contenido = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(contenido))
                Console.WriteLine("El contenido no puede estar vacío. Ingreselo nuevamente.");
        } while (string.IsNullOrWhiteSpace(contenido));

        Post nuevo = new Post(nextId++, titulo, categoria, contenido);
        posts.Add(nuevo);
        Console.WriteLine($"\nPost #{nuevo.Id} creado exitosamente en estado Borrador.");
        Pausar();
    }

    static Post SeleccionarPost(List<Post> lista)
    {
        Post post = null;
        do
        {
            Console.Write("Ingrese ID del post: ");
            string input = Console.ReadLine();
            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine("ID inválido, ingreselo nuevamente.");
                continue;
            }
            post = lista.FirstOrDefault(p => p.Id == id);
            if (post == null)
                Console.WriteLine("Post no encontrado, ingreselo nuevamente.");
        } while (post == null);
        return post;
    }

    static void EditarPost(List<Categoria> categoriasPermitidas)
    {
        Encabezado();
        var editables = posts
            .Where(p => p.Estado == EstadoPost.Borrador &&
                        (categoriasPermitidas == null || categoriasPermitidas.Contains(p.Categoria)))
            .ToList();

        if (!editables.Any())
        {
            Console.WriteLine("No hay posts en Borrador disponibles para editar.");
            Pausar();
            return;
        }

        Console.WriteLine("-- EDITAR POST --\n");
        MostrarLista(editables);
        Post post = SeleccionarPost(editables);

        Console.WriteLine($"\nTítulo actual: {post.Titulo}");
        Console.Write("Nuevo título (Enter para no cambiar): ");
        string nuevoTitulo = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoTitulo))
            post.Titulo = nuevoTitulo;

        List<Categoria> opcionesEdit = categoriasPermitidas ?? Enum.GetValues(typeof(Categoria)).Cast<Categoria>().ToList();
        Console.WriteLine($"Categoría actual: {NombreCategoria(post.Categoria)}");
        Console.WriteLine("Nueva categoría (Enter para no cambiar):");
        for (int i = 0; i < opcionesEdit.Count; i++)
            Console.WriteLine($"  {i + 1}. {NombreCategoria(opcionesEdit[i])}");
        string inputCat;
        while (true)
        {
            Console.Write("Opción: ");
            inputCat = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inputCat)) break;
            if (int.TryParse(inputCat, out int catIdx) && catIdx >= 1 && catIdx <= opcionesEdit.Count)
            {
                post.Categoria = opcionesEdit[catIdx - 1];
                break;
            }
            Console.WriteLine("Opción inválida, ingresela nuevamente (o Enter para no cambiar).");
        }

        Console.WriteLine($"Contenido actual: {post.Contenido}");
        Console.Write("Nuevo contenido (Enter para no cambiar): ");
        string nuevoContenido = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoContenido))
            post.Contenido = nuevoContenido;

        Console.WriteLine("\nPost editado correctamente.");
        Pausar();
    }

    static void PublicarPost()
    {
        Encabezado();
        var publicables = posts.Where(p => p.Estado == EstadoPost.Borrador).ToList();

        if (!publicables.Any())
        {
            Console.WriteLine("No hay posts en Borrador para publicar.");
            Pausar();
            return;
        }

        Console.WriteLine("-- PUBLICAR POST --\n");
        MostrarLista(publicables);
        Post post = SeleccionarPost(publicables);

        post.Publicar();
        Console.WriteLine($"\nPost #{post.Id} publicado el {post.FechaPublicacion:dd/MM/yyyy HH:mm}.");
        Pausar();
    }

    static void ArchivarPost()
    {
        Encabezado();
        var archivables = posts.Where(p => p.Estado == EstadoPost.Publicado).ToList();

        if (!archivables.Any())
        {
            Console.WriteLine("No hay posts publicados para archivar.");
            Pausar();
            return;
        }

        Console.WriteLine("-- ARCHIVAR POST --\n");
        MostrarLista(archivables);
        Post post = SeleccionarPost(archivables);

        post.Archivar();
        Console.WriteLine($"\nPost #{post.Id} archivado.");
        Pausar();
    }

    static void EliminarPost()
    {
        Encabezado();
        var eliminables = posts.Where(p => p.Estado != EstadoPost.Eliminado).ToList();

        if (!eliminables.Any())
        {
            Console.WriteLine("No hay posts disponibles para eliminar.");
            Pausar();
            return;
        }

        Console.WriteLine("-- ELIMINAR POST --\n");
        MostrarLista(eliminables);
        Post post = SeleccionarPost(eliminables);

        Console.Write("\n¿Confirmar eliminación? Esta acción es irreversible. (s/n): ");
        if (Console.ReadLine().Trim().ToLower() == "s")
        {
            post.Eliminar();
            Console.WriteLine($"Post #{post.Id} eliminado definitivamente.");
        }
        else
        {
            Console.WriteLine("Operación cancelada.");
        }
        Pausar();
    }

    static void ScrollearPosts()
    {
        var publicados = posts.Where(p => p.Estado == EstadoPost.Publicado).ToList();

        if (!publicados.Any())
        {
            Console.WriteLine("No hay posts publicados para ver.");
            Pausar();
            return;
        }

        int index = 0;
        while (index < publicados.Count)
        {
            Encabezado();
            Post post = publicados[index];
            Console.WriteLine($"Post {index + 1} de {publicados.Count}");
            Console.WriteLine("─────────────────────────────────");
            Console.WriteLine($"Título:    {post.Titulo}");
            Console.WriteLine($"Categoría: {NombreCategoria(post.Categoria)}");
            Console.WriteLine($"Contenido: {post.Contenido}");
            Console.WriteLine($"Likes:     {post.Likes}");
            Console.WriteLine("─────────────────────────────────");
            Console.WriteLine();

            Console.Write("¿Dar like? (s/n): ");
            if (Console.ReadLine().Trim().ToLower() == "s")
            {
                post.AgregarLike();
                Console.WriteLine("¡Like enviado!");
            }

            index++;

            if (index < publicados.Count)
            {
                Console.Write("¿Ver siguiente post? (s/n): ");
                if (Console.ReadLine().Trim().ToLower() != "s")
                    break;
            }
        }

        Console.WriteLine("\n¡Hasta la próxima, tortuga!");
        Pausar();
    }

    // ===================== INFORMES =====================

    static void EmitirInformes()
    {
        InformeActividad();
        InformeRendimiento();
        InformeAuditoria();
    }

    static void InformeActividad()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE ACTIVIDAD (1/3) ===\n");

        Console.WriteLine("-- Posts por estado --");
        foreach (EstadoPost estado in Enum.GetValues(typeof(EstadoPost)))
        {
            int count = posts.Count(p => p.Estado == estado);
            Console.WriteLine($"  {estado}: {count} post(s)");
        }

        Console.WriteLine("\n-- Posts por categoría (excluye eliminados) --");
        foreach (Categoria cat in Enum.GetValues(typeof(Categoria)))
        {
            int count = posts.Count(p => p.Categoria == cat && p.Estado != EstadoPost.Eliminado);
            Console.WriteLine($"  {NombreCategoria(cat)}: {count} post(s)");
        }

        Pausar();
    }

    static void InformeRendimiento()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE RENDIMIENTO (2/3) ===\n");

        var publicados = posts.Where(p => p.Estado == EstadoPost.Publicado).ToList();

        if (!publicados.Any())
        {
            Console.WriteLine("No hay posts publicados.");
            Pausar();
            return;
        }

        var top3 = publicados.OrderByDescending(p => p.Likes).Take(3).ToList();

        Console.WriteLine("Top 3 posts con más likes:");
        for (int i = 0; i < top3.Count; i++)
            Console.WriteLine($"  {i + 1}. [{top3[i].Id}] {top3[i].Titulo} — {top3[i].Likes} likes");

        int totalLikes = publicados.Sum(p => p.Likes);
        Console.WriteLine($"\nTotal de likes acumulados en posts publicados: {totalLikes}");

        Pausar();
    }

    static void InformeAuditoria()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE AUDITORÍA (3/3) ===\n");

        var eliminados = posts.Where(p => p.Estado == EstadoPost.Eliminado).ToList();

        if (!eliminados.Any())
        {
            Console.WriteLine("No hay posts eliminados.");
            Pausar();
            return;
        }

        Console.WriteLine("Posts eliminados:");
        foreach (var post in eliminados)
            Console.WriteLine($"  [{post.Id}] {post.Titulo} | {NombreCategoria(post.Categoria)} | Eliminado: {post.FechaEliminacion:dd/MM/yyyy HH:mm}");

        Pausar();
    }

    // ===================== UTILIDADES =====================

    static string NombreCategoria(Categoria cat)
    {
        switch (cat)
        {
            case Categoria.Faranews: return "Faranews";
            case Categoria.Opinion: return "Opinión";
            case Categoria.LaCancelacion: return "La cancelación";
            case Categoria.Tatianas: return "Tatianas";
            case Categoria.Nereas: return "Nereas";
            default: return cat.ToString();
        }
    }

    static void MostrarLista(List<Post> lista)
    {
        foreach (var post in lista)
            Console.WriteLine($"  [{post.Id}] {post.Titulo} | {NombreCategoria(post.Categoria)} | Estado: {post.Estado} | Likes: {post.Likes}");
        Console.WriteLine();
    }

    static void Pausar()
    {
        Console.WriteLine("\nPresione Enter para continuar...");
        Console.ReadLine();
    }
}

public enum Categoria
{
    Faranews,
    Opinion,
    LaCancelacion,
    Tatianas,
    Nereas
}

public enum EstadoPost
{
    Borrador,
    Publicado,
    Archivado,
    Eliminado
}

public class Post
{
    public int Id { get; private set; }
    public string Titulo { get; set; }
    public Categoria Categoria { get; set; }
    public string Contenido { get; set; }
    public int Likes { get; private set; }
    public EstadoPost Estado { get; private set; }
    public DateTime? FechaPublicacion { get; private set; }
    public DateTime? FechaEliminacion { get; private set; }

    public Post(int id, string titulo, Categoria categoria, string contenido)
    {
        Id = id;
        Titulo = titulo;
        Categoria = categoria;
        Contenido = contenido;
        Likes = 0;
        Estado = EstadoPost.Borrador;
        FechaPublicacion = null;
        FechaEliminacion = null;
    }

    public void AgregarLike()
    {
        Likes++;
    }

    public void Publicar()
    {
        Estado = EstadoPost.Publicado;
        FechaPublicacion = DateTime.Now;
    }

    public void Archivar()
    {
        Estado = EstadoPost.Archivado;
    }

    public void Eliminar()
    {
        Estado = EstadoPost.Eliminado;
        FechaEliminacion = DateTime.Now;
    }
}
