using System;
using System.Collections.Generic;
using System.Linq;

// Interfaces
interface IUsuario
{
    string Nombre { get; set; }
    string Email { get; set; }
    string Contraseña { get; set; }
    bool IniciarSesion(string email, string contraseña);
}

interface IDirector : IUsuario
{
    List<Profesor> VerListadoProfesores();
    List<Estudiante> VerListadoEstudiantesPorGrado(int grado);
    void AgregarProfesor(Profesor profesor);
    void AgregarEstudiante(Estudiante estudiante);
}

interface IProfesor : IUsuario
{
    List<Estudiante> VerListadoEstudiantesPorGrado(int grado);
    void AgregarContenidoClase(int grado, string materia, string contenido);
    void AgregarEstudiante(Estudiante estudiante);
}

interface IEstudiante : IUsuario
{
    List<Profesor> VerListadoProfesores();
    string VerContenidoClase(string materia);
    void AgregarProfesor(Profesor profesor);
}

// Clases
class Escuela
{
    private List<Profesor> profesores = new List<Profesor>();
    private List<Estudiante> estudiantes = new List<Estudiante>();
    private Director director;

    public Escuela(Director director)
    {
        this.director = director;
    }

    public void AgregarProfesor(Profesor profesor)
    {
        profesores.Add(profesor);
    }

    public void AgregarEstudiante(Estudiante estudiante)
    {
        estudiantes.Add(estudiante);
    }

    public IUsuario IniciarSesion(string email, string contraseña)
    {
        if (director.IniciarSesion(email, contraseña))
            return director;

        foreach (Profesor profesor in profesores)
        {
            if (profesor.IniciarSesion(email, contraseña))
                return profesor;
        }

        foreach (Estudiante estudiante in estudiantes)
        {
            if (estudiante.IniciarSesion(email, contraseña))
                return estudiante;
        }

        return null;
    }
}

class Director : IDirector
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Contraseña { get; set; }

    private List<Profesor> ListaProfesores = new List<Profesor>();
    private List<Estudiante> ListaEstudiantes = new List<Estudiante>();

    public bool IniciarSesion(string email, string contraseña)
    {
        return Email == email && Contraseña == contraseña;
    }

    public List<Profesor> VerListadoProfesores()
    {
        return ListaProfesores;
    }

    public List<Estudiante> VerListadoEstudiantesPorGrado(int grado)
    {
        return ListaEstudiantes.Where(e => e.Grado == grado).ToList();
    }

    public void AgregarProfesor(Profesor profesor)
    {
        ListaProfesores.Add(profesor);
    }

    public void AgregarEstudiante(Estudiante estudiante)
    {
        ListaEstudiantes.Add(estudiante);
    }
}

class Profesor : IProfesor
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Contraseña { get; set; }

    public Dictionary<int, Dictionary<string, List<string>>> contenidosClase = new Dictionary<int, Dictionary<string, List<string>>>();
    private List<Estudiante> ListaEstudiantes = new List<Estudiante>();

    public bool IniciarSesion(string email, string contraseña)
    {
        return Email == email && Contraseña == contraseña;
    }

    public List<Estudiante> VerListadoEstudiantesPorGrado(int grado)
    {
        return ListaEstudiantes.Where(e => e.Grado == grado).ToList();
    }

    public void AgregarContenidoClase(int grado, string materia, string contenido)
    {
        if (!contenidosClase.ContainsKey(grado))
            contenidosClase[grado] = new Dictionary<string, List<string>>();

        if (!contenidosClase[grado].ContainsKey(materia))
            contenidosClase[grado][materia] = new List<string>();

        contenidosClase[grado][materia].Add(contenido);
    }

    public void AgregarEstudiante(Estudiante estudiante)
    {
        ListaEstudiantes.Add(estudiante);
    }
}

class Estudiante : IEstudiante
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Contraseña { get; set; }
    public int Grado { get; set; }

    private List<Profesor> ListaProfesores = new List<Profesor>();

    public bool IniciarSesion(string email, string contraseña)
    {
        return Email == email && Contraseña == contraseña;
    }

    public List<Profesor> VerListadoProfesores()
    {
        return ListaProfesores;
    }

    public string VerContenidoClase(string materia)
    {
        foreach (Profesor profesor in ListaProfesores)
        {
            if (profesor.contenidosClase.ContainsKey(Grado) && profesor.contenidosClase[Grado].ContainsKey(materia))
            {
                return string.Join(Environment.NewLine, profesor.contenidosClase[Grado][materia]);
            }
        }

        return string.Empty;
    }

    public void AgregarProfesor(Profesor profesor)
    {
        ListaProfesores.Add(profesor);
    }
}

// Programa principal
class Program
{
    static void Main(string[] args)
    {
        Director director = new Director { Nombre = "Director", Email = "director@escuela.com", Contraseña = "123456" };
        Escuela escuela = new Escuela(director);

        Profesor profesor = new Profesor { Nombre = "Profesor", Email = "profesor@escuela.com", Contraseña = "123456" };
        escuela.AgregarProfesor(profesor);

        Estudiante estudiante = new Estudiante { Nombre = "Estudiante", Email = "estudiante@escuela.com", Contraseña = "123456", Grado = 5 };
        escuela.AgregarEstudiante(estudiante);

        profesor.AgregarEstudiante(estudiante);
        estudiante.AgregarProfesor(profesor);

        profesor.AgregarContenidoClase(5, "Matemáticas", "Suma y resta");

        Console.WriteLine("Ingrese su correo electrónico:");
        string email = Console.ReadLine();
        Console.WriteLine("Ingrese su contraseña:");
        string contraseña = Console.ReadLine();

        IUsuario usuario = escuela.IniciarSesion(email, contraseña);
        if (usuario != null)
        {
            if (usuario is Director)
            {
                DirectorMenu((Director)usuario, escuela);
            }
            else if (usuario is Profesor)
            {
                ProfesorMenu((Profesor)usuario);
            }
            else if (usuario is Estudiante)
            {
                EstudianteMenu((Estudiante)usuario);
            }
        }
        else
        {
            Console.WriteLine("Correo electrónico o contraseña incorrectos.");
        }
    }

    static void DirectorMenu(Director director, Escuela escuela)
    {
        while (true)
        {
            Console.WriteLine("\n--- Menú Director ---");
            Console.WriteLine("1. Ver listado de profesores");
            Console.WriteLine("2. Agregar profesor");
            Console.WriteLine("3. Ver listado de estudiantes por grado");
            Console.WriteLine("4. Agregar estudiante");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.WriteLine("Listado de Profesores:");
                    foreach (Profesor p in director.VerListadoProfesores())
                    {
                        Console.WriteLine("- " + p.Nombre);
                    }
                    break;
                case "2":
                    Console.Write("Nombre del profesor: ");
                    string nombreProfesor = Console.ReadLine();
                    Console.Write("Email del profesor: ");
                    string emailProfesor = Console.ReadLine();
                    Console.Write("Contraseña del profesor: ");
                    string contraseñaProfesor = Console.ReadLine();
                    Profesor nuevoProfesor = new Profesor { Nombre = nombreProfesor, Email = emailProfesor, Contraseña = contraseñaProfesor };
                    director.AgregarProfesor(nuevoProfesor);
                    escuela.AgregarProfesor(nuevoProfesor);
                    Console.WriteLine("Profesor agregado exitosamente.");
                    break;
                case "3":
                    Console.Write("Ingrese el grado: ");
                    int grado = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Listado de Estudiantes del grado {grado}:");
                    foreach (Estudiante e in director.VerListadoEstudiantesPorGrado(grado))
                    {
                        Console.WriteLine("- " + e.Nombre);
                    }
                    break;
                case "4":
                    Console.Write("Nombre del estudiante: ");
                    string nombreEstudiante = Console.ReadLine();
                    Console.Write("Email del estudiante: ");
                    string emailEstudiante = Console.ReadLine();
                    Console.Write("Contraseña del estudiante: ");
                    string contraseñaEstudiante = Console.ReadLine();
                    Console.Write("Grado del estudiante: ");
                    int gradoEstudiante = int.Parse(Console.ReadLine());
                    Estudiante nuevoEstudiante = new Estudiante { Nombre = nombreEstudiante, Email = emailEstudiante, Contraseña = contraseñaEstudiante, Grado = gradoEstudiante };
                    director.AgregarEstudiante(nuevoEstudiante);
                    escuela.AgregarEstudiante(nuevoEstudiante);
                    Console.WriteLine("Estudiante agregado exitosamente.");
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }
        }
    }

    static void ProfesorMenu(Profesor profesor)
    {
        while (true)
        {
            Console.WriteLine("\n--- Menú Profesor ---");
            Console.WriteLine("1. Ver listado de estudiantes por grado");
            Console.WriteLine("2. Agregar contenido de clase");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.Write("Ingrese el grado: ");
                    int grado = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Listado de Estudiantes del grado {grado}:");
                    foreach (Estudiante e in profesor.VerListadoEstudiantesPorGrado(grado))
                    {
                        Console.WriteLine("- " + e.Nombre);
                    }
                    break;
                case "2":
                    Console.Write("Ingrese el grado: ");
                    int gradoClase = int.Parse(Console.ReadLine());
                    Console.Write("Ingrese la materia: ");
                    string materia = Console.ReadLine();
                    Console.Write("Ingrese el contenido: ");
                    string contenido = Console.ReadLine();
                    profesor.AgregarContenidoClase(gradoClase, materia, contenido);
                    Console.WriteLine("Contenido de clase agregado exitosamente.");
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }
        }
    }

    static void EstudianteMenu(Estudiante estudiante)
    {
        while (true)
        {
            Console.WriteLine("\n--- Menú Estudiante ---");
            Console.WriteLine("1. Ver listado de profesores");
            Console.WriteLine("2. Ver contenido de clase");
            Console.WriteLine("3. Salir");
            Console.Write("Seleccione una opción: ");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    Console.WriteLine("Listado de Profesores:");
                    foreach (Profesor p in estudiante.VerListadoProfesores())
                    {
                        Console.WriteLine("- " + p.Nombre);
                    }
                    break;
                case "2":
                    Console.Write("Ingrese la materia: ");
                    string materia = Console.ReadLine();
                    string contenido = estudiante.VerContenidoClase(materia);
                    if (string.IsNullOrEmpty(contenido))
                    {
                        Console.WriteLine("No hay contenido disponible para esta materia.");
                    }
                    else
                    {
                        Console.WriteLine("Contenido de la clase de " + materia + ":");
                        Console.WriteLine(contenido);
                    }
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                    break;
            }
        }
    }
}
