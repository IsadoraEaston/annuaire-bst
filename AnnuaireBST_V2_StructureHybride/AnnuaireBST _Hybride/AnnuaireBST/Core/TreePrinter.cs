using AnnuaireBST.Models;

namespace AnnuaireBST.Core;

/// <summary>
/// Dessine un arbre binaire de recherche (BST) avec l’algorithme Reingold‑Tilford.
/// Résultat : centré, couleurs par niveau, traits obliques, pas de chevauchement.
/// </summary>
public static class TreePrinter
{
    // Cinq couleurs qui se répètent pour différencier les niveaux (profondeur)
    private static readonly ConsoleColor[] levelColors = new ConsoleColor[]
    {
        ConsoleColor.Cyan,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.Magenta,
        ConsoleColor.Blue,
        ConsoleColor.Red
    };

    // Compression horizontale : 1.0 = espacement naturel, 0.8 = plus serré
    private const double COMPRESSION = 0.8;

    // Distance minimale entre deux sous‑arbres (en caractères)
    private const double SEPARATION_MIN = 3;

    // ===================== POINT D'ENTRÉE PRINCIPAL =====================
    public static void AfficherArbre(BSTNode<Contact>? racine)
    {
        if (racine == null) return;

        // 1. Construire la structure de mise en page (coordonnées relatives)
        var miseEnPage = ConstruireMiseEnPage(racine);
        if (miseEnPage == null) return;

        // 2. Additionner les décalages (Mod) pour obtenir les coordonnées absolues
        AppliquerDecalages(miseEnPage, 0);

        // 3. Rendre l’arbre plus ou moins large selon la compression choisie
        CompresserHorizontalement(miseEnPage);

        // Récupérer tous les nœuds avec leurs positions finales
        var tousLesNoeuds = new List<NoeudDeMiseEnPage>();
        CollecterNoeuds(miseEnPage, tousLesNoeuds);
        int xMin = tousLesNoeuds.Min(n => n.X);
        int xMax = tousLesNoeuds.Max(n => n.X);

        // Centrer l’arbre dans la fenêtre de la console
        int centreConsole = Console.WindowWidth / 2;
        int centreArbre = (xMin + xMax) / 2;
        int decalageX = centreConsole - centreArbre;
        if (xMin + decalageX < 2) decalageX = 2 - xMin;

        // Agrandir la zone de la console si l’arbre est trop large
        int largeurNecessaire = xMax - xMin + 20;
        if (largeurNecessaire > Console.BufferWidth)
            Console.BufferWidth = largeurNecessaire;

        Console.Clear();

        // --- Étape A : afficher les noms des nœuds (avec une espace à la fin pour ne pas coller)
        foreach (var nd in tousLesNoeuds)
        {
            int ecranX = nd.X + decalageX;
            int ecranY = nd.Profondeur * 2 + 3;
            if (ecranX >= 0 && ecranX < Console.BufferWidth && ecranY >= 0 && ecranY < Console.BufferHeight)
            {
                Console.SetCursorPosition(ecranX, ecranY);
                Console.ForegroundColor = levelColors[nd.Profondeur % levelColors.Length];
                Console.Write(nd.NoeudReel?.Value.Name + " ");
            }
        }

        // --- Étape B : dessiner les traits obliques '/' ou '\' entre parent et enfant
        foreach (var nd in tousLesNoeuds)
        {
            if (nd.Parent != null)
                DessinerTrait(nd.Parent, nd, decalageX);
        }

        // Reposer le curseur sous l’arbre pour ne pas gâcher l’affichage suivant
        Console.SetCursorPosition(0, tousLesNoeuds.Max(n => n.Profondeur) * 2 + 5);
    }

    // ===================== DESSIN D'UN TRAIT =====================
    private static void DessinerTrait(NoeudDeMiseEnPage parent, NoeudDeMiseEnPage enfant, int decalageX)
    {
        int parentEcranX = parent.X + decalageX;
        int parentEcranY = parent.Profondeur * 2 + 3;
        int enfantEcranX = enfant.X + decalageX;
        int enfantEcranY = enfant.Profondeur * 2 + 3;

        int longueurNomParent = parent.NoeudReel?.Value.Name.Length ?? 0;
        int longueurNomEnfant = enfant.NoeudReel?.Value.Name.Length ?? 0;
        int centreParent = parentEcranX + longueurNomParent / 2;
        int centreEnfant = enfantEcranX + longueurNomEnfant / 2;

        int ligneY = parentEcranY + 1;                     // la ligne du milieu entre parent et enfant
        char barre = (enfant == parent.Gauche) ? '/' : '\\';
        int ligneX = (centreParent + centreEnfant) / 2;
        ligneX = Math.Clamp(ligneX, 0, Console.BufferWidth - 1);

        if (ligneY >= 0 && ligneY < Console.BufferHeight)
        {
            Console.SetCursorPosition(ligneX, ligneY);
            Console.Write(barre);
        }
    }

    // ===================== COMPRESSION HORIZONTALE =====================
    private static void CompresserHorizontalement(NoeudDeMiseEnPage nd)
    {
        if (nd == null) return;
        nd.X = (int)(nd.X * COMPRESSION);
        CompresserHorizontalement(nd.Gauche);
        CompresserHorizontalement(nd.Droite);
    }

    // ===================== STRUCTURE DE MISE EN PAGE (ALGORITHME R‑T) =====================

    /// <summary> Nœud utilitaire qui garde les infos de placement (X, profondeur, largeur, etc.) </summary>
    private class NoeudDeMiseEnPage
    {
        public BSTNode<Contact>? NoeudReel;    // le vrai nœud de l’annuaire
        public int Largeur;                    // longueur du nom + 1 (une espace)
        public int X;                          // position horizontale calculée
        public int Profondeur;                 // niveau dans l’arbre (0 pour la racine)
        public int DecalageMod;                // pour l’algorithme (non utilisé ici)
        public NoeudDeMiseEnPage? Gauche;
        public NoeudDeMiseEnPage? Droite;
        public NoeudDeMiseEnPage? Parent;

        public NoeudDeMiseEnPage(BSTNode<Contact>? noeud, int profondeur)
        {
            NoeudReel = noeud;
            Profondeur = profondeur;
            // on ajoute 1 pour la petite espace qu’on mettra après le nom
            Largeur = (noeud?.Value.Name.Length ?? 0) + 1;
        }
    }

    private static NoeudDeMiseEnPage? ConstruireMiseEnPage(BSTNode<Contact>? racine)
    {
        if (racine == null) return null;
        NoeudDeMiseEnPage mise = new NoeudDeMiseEnPage(racine, 0);
        CalculerMiseEnPageRecursif(mise);
        return mise;
    }

    /// <summary> Parcours post‑ordre : on calcule les positions en commençant par les feuilles </summary>
    private static void CalculerMiseEnPageRecursif(NoeudDeMiseEnPage nd)
    {
        if (nd.NoeudReel == null) return;

        // Construire les sous‑arbres gauche et droit
        if (nd.NoeudReel.Left != null)
        {
            nd.Gauche = new NoeudDeMiseEnPage(nd.NoeudReel.Left, nd.Profondeur + 1);
            nd.Gauche.Parent = nd;
            CalculerMiseEnPageRecursif(nd.Gauche);
        }
        if (nd.NoeudReel.Right != null)
        {
            nd.Droite = new NoeudDeMiseEnPage(nd.NoeudReel.Right, nd.Profondeur + 1);
            nd.Droite.Parent = nd;
            CalculerMiseEnPageRecursif(nd.Droite);
        }

        // --- Cas 1 : feuille (pas d’enfant) ---
        if (nd.Gauche == null && nd.Droite == null)
        {
            nd.X = 0;
            return;
        }

        // --- Cas 2 : un seul enfant ---
        if (nd.Gauche != null && nd.Droite == null)
        {
            double decalage = nd.Largeur / 2.0 + SEPARATION_MIN;
            nd.Gauche.X += (int)decalage;
            nd.X = nd.Gauche.X - (int)decalage;
            return;
        }
        if (nd.Droite != null && nd.Gauche == null)
        {
            double decalage = nd.Largeur / 2.0 + SEPARATION_MIN;
            nd.Droite.X -= (int)decalage;
            nd.X = nd.Droite.X + (int)decalage;
            return;
        }

        // --- Cas 3 : deux enfants → il faut les écarter pour qu’ils ne se touchent pas ---
        var contourGauche = CalculerContour(nd.Gauche!);
        var contourDroit = CalculerContour(nd.Droite!);

        // De combien faut‑il pousser le sous‑arbre droit vers la droite ?
        double decalageNecessaire = (contourGauche.MaxX + SEPARATION_MIN) - contourDroit.MinX;
        if (decalageNecessaire > 0)
            DecalerSousArbre(nd.Droite!, (int)decalageNecessaire);

        // Recalculer les contours après ce décalage
        contourGauche = CalculerContour(nd.Gauche!);
        contourDroit = CalculerContour(nd.Droite!);

        // Le parent se place au milieu entre le bord droit du sous‑arbre gauche
        // et le bord gauche du sous‑arbre droit
        nd.X = (contourGauche.MaxX + contourDroit.MinX) / 2;
    }

    /// <summary> Renvoie le x minimum et le x maximum de tout le sous‑arbre (y compris les descendants) </summary>
    private static (int MinX, int MaxX) CalculerContour(NoeudDeMiseEnPage nd)
    {
        int min = nd.X;
        int max = nd.X + nd.Largeur;
        if (nd.Gauche != null)
        {
            var gauche = CalculerContour(nd.Gauche);
            min = Math.Min(min, gauche.MinX);
            max = Math.Max(max, gauche.MaxX);
        }
        if (nd.Droite != null)
        {
            var droite = CalculerContour(nd.Droite);
            min = Math.Min(min, droite.MinX);
            max = Math.Max(max, droite.MaxX);
        }
        return (min, max);
    }

    /// <summary> Ajoute une valeur delta à tous les nœuds d’un sous‑arbre (pour écarter) </summary>
    private static void DecalerSousArbre(NoeudDeMiseEnPage nd, int delta)
    {
        if (nd == null) return;
        nd.X += delta;
        DecalerSousArbre(nd.Gauche, delta);
        DecalerSousArbre(nd.Droite, delta);
    }

    /// <summary> Applique la somme des décalages (Mod) à tous les nœuds (parcours préfixe) </summary>
    private static void AppliquerDecalages(NoeudDeMiseEnPage? nd, int sommeMod)
    {
        if (nd == null) return;
        nd.X += sommeMod;
        AppliquerDecalages(nd.Gauche, sommeMod + nd.DecalageMod);
        AppliquerDecalages(nd.Droite, sommeMod + nd.DecalageMod);
    }

    private static void CollecterNoeuds(NoeudDeMiseEnPage? nd, List<NoeudDeMiseEnPage> liste)
    {
        if (nd == null) return;
        liste.Add(nd);
        CollecterNoeuds(nd.Gauche, liste);
        CollecterNoeuds(nd.Droite, liste);
    }
}