using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the basis for the core AI logic
// simply AI actions will rely on 2 factors, grid position of current AI unit and an action value which will be calculated with parameters decided for each action per script
// position and values will be sorted and this sort will determine the best action for each AI target at a given time
// the benefits to this is it can be easily modified per class to tweak for difficulty
// downside would be scaling this for difficulty settings (easy, medium, hard) or as the game progresses (more desparate AI) will require more adjustment, also can become more predictable.
public class EnemyAIAction 
{
    public GridPosition gridPosition;
    public int actionValue; 
}
