
public class WarriorAltarBurner : PowerableObject {

    public int id; 

	protected override void DoYourPowerableThing ()
	{
        if (id != 0)
        {
            BurnableObject[] bObjects = FindObjectsOfType<BurnableObject>();
            foreach (BurnableObject bObject in bObjects)
            {
                if (bObject.gameObject.CompareTag("BurnableBranches" + id.ToString()))
                {
                    bObject.Burn();
                }
            }
        }
	}
}
