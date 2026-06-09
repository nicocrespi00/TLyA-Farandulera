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
            Console.WriteLine("0. Volver");
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
            Console.WriteLine("0. Volver");
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
            Console.WriteLine("2. Informes de gestión");
            Console.WriteLine("0. Volver");
            Console.WriteLine();
            Console.Write("Opción: ");

            switch (Console.ReadLine())
            {
                case "1": EliminarPost(); break;
                case "2": MenuInformes(); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); Pausar(); break;
            }
        }
    }

    static void MenuInformes()
    {
        bool salir = false;
        while (!salir)
        {
            Encabezado();
            Console.WriteLine("-- INFORMES --");
            Console.WriteLine("1. Informe de actividad (por estado y categoría)");
            Console.WriteLine("2. Informe de rendimiento (top 3 posts con más likes)");
            Console.WriteLine("3. Informe de auditoría (posts eliminados)");
            Console.WriteLine("0. Volver");
            Console.WriteLine();
            Console.Write("Opción: ");

            switch (Console.ReadLine())
            {
                case "1": InformeActividad(); break;
                case "2": InformeRendimiento(); break;
                case "3": InformeAuditoria(); break;
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

        Console.Write("Título: ");
        string titulo = Console.ReadLine();

        List<Categoria> opciones = categoriasPermitidas ?? Enum.GetValues(typeof(Categoria)).Cast<Categoria>().ToList();
        Console.WriteLine("Categoría:");
        for (int i = 0; i < opciones.Count; i++)
            Console.WriteLine($"  {i + 1}. {opciones[i]}");
        Console.Write("Opción: ");

        if (!int.TryParse(Console.ReadLine(), out int catIdx) || catIdx < 1 || catIdx > opciones.Count)
        {
            Console.WriteLine("Categoría inválida.");
            Pausar();
            return;
        }
        Categoria categoria = opciones[catIdx - 1];

        Console.Write("Contenido: ");
        string contenido = Console.ReadLine();

        Post nuevo = new Post(nextId++, titulo, categoria, contenido);
        posts.Add(nuevo);
        Console.WriteLine($"\nPost #{nuevo.Id} creado exitosamente en estado Borrador.");
        Pausar();
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
        Console.Write("Ingrese ID del post a editar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Post post = editables.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            Console.WriteLine("Post no encontrado o sin permiso.");
            Pausar();
            return;
        }

        Console.WriteLine($"\nTítulo actual: {post.Titulo}");
        Console.Write("Nuevo título (Enter para no cambiar): ");
        string nuevoTitulo = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(nuevoTitulo))
            post.Titulo = nuevoTitulo;

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
        Console.Write("Ingrese ID del post a publicar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Post post = publicables.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            Console.WriteLine("Post no encontrado.");
            Pausar();
            return;
        }

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
        Console.Write("Ingrese ID del post a archivar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Post post = archivables.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            Console.WriteLine("Post no encontrado.");
            Pausar();
            return;
        }

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
        Console.Write("Ingrese ID del post a eliminar: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido.");
            Pausar();
            return;
        }

        Post post = eliminables.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            Console.WriteLine("Post no encontrado.");
            Pausar();
            return;
        }

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
            Console.WriteLine($"Título: {post.Titulo}");
            Console.WriteLine($"Categoría: {post.Categoria}");
            Console.WriteLine($"Contenido: {post.Contenido}");
            Console.WriteLine($"Likes: {post.Likes}");
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

    static void InformeActividad()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE ACTIVIDAD ===\n");

        Console.WriteLine("-- Cantidad de posts por estado --");
        foreach (EstadoPost estado in Enum.GetValues(typeof(EstadoPost)))
        {
            int count = posts.Count(p => p.Estado == estado);
            Console.WriteLine($"  {estado}: {count} post(s)");
        }

        Console.WriteLine("\n-- Cantidad de posts por categoría (excluye eliminados) --");
        foreach (Categoria cat in Enum.GetValues(typeof(Categoria)))
        {
            int count = posts.Count(p => p.Categoria == cat && p.Estado != EstadoPost.Eliminado);
            Console.WriteLine($"  {cat}: {count} post(s)");
        }

        Pausar();
    }

    static void InformeRendimiento()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE RENDIMIENTO ===\n");

        var top3 = posts
            .Where(p => p.Estado == EstadoPost.Publicado)
            .OrderByDescending(p => p.Likes)
            .Take(3)
            .ToList();

        if (!top3.Any())
        {
            Console.WriteLine("No hay posts publicados.");
            Pausar();
            return;
        }

        Console.WriteLine("Top 3 posts con más likes:");
        for (int i = 0; i < top3.Count; i++)
            Console.WriteLine($"  {i + 1}. [{top3[i].Id}] {top3[i].Titulo} — {top3[i].Likes} likes");

        int totalLikes = top3.Sum(p => p.Likes);
        Console.WriteLine($"\nTotal de likes acumulados: {totalLikes}");

        Pausar();
    }

    static void InformeAuditoria()
    {
        Encabezado();
        Console.WriteLine("=== INFORME DE AUDITORÍA ===\n");

        var eliminados = posts.Where(p => p.Estado == EstadoPost.Eliminado).ToList();

        if (!eliminados.Any())
        {
            Console.WriteLine("No hay posts eliminados.");
            Pausar();
            return;
        }

        Console.WriteLine("Posts eliminados:");
        foreach (var post in eliminados)
            Console.WriteLine($"  [{post.Id}] {post.Titulo} | {post.Categoria} | Eliminado: {post.FechaEliminacion:dd/MM/yyyy HH:mm}");

        Pausar();
    }

    // ===================== UTILIDADES =====================

    static void MostrarLista(List<Post> lista)
    {
        foreach (var post in lista)
            post.MostrarResumen();
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

    public void MostrarResumen()
    {
        Console.WriteLine($"  [{Id}] {Titulo} | {Categoria} | Estado: {Estado} | Likes: {Likes}");
        if (FechaPublicacion.HasValue)
            Console.WriteLine($"       Publicado: {FechaPublicacion.Value:dd/MM/yyyy HH:mm}");
    }
}
