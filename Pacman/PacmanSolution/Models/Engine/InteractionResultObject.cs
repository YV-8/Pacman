namespace PacmanSolution.Models;

public class InteractionResultObject
{
    /// <summary>
    /// Is the points get something 
    /// </summary>
    public int PointsEarned { get; set; }
    
    /// <summary>
    /// the type element who want delete
    /// </summary>
    public string? RemovedElementType { get; set; }
    /// <summary>
    /// the interaction was success
    /// </summary>
    public bool Success { get; set; }
}