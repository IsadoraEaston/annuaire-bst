using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnuaireBST.DataStructures;
using AnnuaireBST.Models;

namespace AnnuaireBST.Core;

public static class Benchmark
{
    // Mesure le temps d'exécution des 4 traversées
    // Complexité : O(n) pour chaque traversée
    public static void MesurerTraversees(BST<Contact> arbre)
    {
        Console.WriteLine("\n══════════════════════════════════════════════════");
        Console.WriteLine("  COMPARATIF DES PARCOURS DE L'ARBRE BST");
        Console.WriteLine("══════════════════════════════════════════════════");
        Console.WriteLine($"  {"Parcours",-15} {"Temps (ms)",-15} {"Complexité"}");
        Console.WriteLine($"  {new string('-', 45)}");

        MesurerEtAfficher("PreOrder", () => arbre.PreOrder(), "O(n)");
        MesurerEtAfficher("InOrder", () => arbre.InOrder(), "O(n)");
        MesurerEtAfficher("PostOrder", () => arbre.PostOrder(), "O(n)");
        MesurerEtAfficher("BFS", () => arbre.BFS(), "O(n)");

        Console.WriteLine($"\n  n = nombre de noeuds | h = hauteur de l'arbre");
        Console.WriteLine("══════════════════════════════════════════════════");
    }

    private static void MesurerEtAfficher(string nom, Action action, string complexite)
    {
        // Rediriger la sortie console pour ne pas afficher les contacts
        var originalOut = Console.Out;
        Console.SetOut(TextWriter.Null);

        Stopwatch sw = Stopwatch.StartNew();
        action();
        sw.Stop();

        Console.SetOut(originalOut);
        Console.WriteLine($"  {nom,-15} {sw.Elapsed.TotalMilliseconds.ToString("F4"),-15} {complexite}");
    }
}
