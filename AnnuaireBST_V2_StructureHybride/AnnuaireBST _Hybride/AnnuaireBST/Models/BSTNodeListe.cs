namespace AnnuaireBST.Models;

/// <summary>
/// Noeud d'un BST dont chaque noeud contient une liste de contacts
/// Permet de gerer les doublons de nom — chaque noeud peut avoir plusieurs contacts
/// </summary>
public class BSTNodeListe<K, V>
{
    public K Cle;                    // la cle de tri (ex: nom)
    public List<V> Valeurs;          // liste des contacts avec cette cle
    public BSTNodeListe<K, V> Left;
    public BSTNodeListe<K, V> Right;

    // Propriete de commodite
    public bool IsLeaf => Left == null && Right == null;

    public BSTNodeListe(K cle, V valeur)
    {
        Cle = cle;
        Valeurs = new List<V>();
        Valeurs.Add(valeur);
        Left = Right = null;
    }
}