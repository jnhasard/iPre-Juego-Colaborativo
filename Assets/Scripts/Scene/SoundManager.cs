using UnityEngine;

/**
 * Store the AudioClips names here.
 */
public class GameSounds
{

    // Players
    public static string PlayerDie;
    public static string PlayerJump = "Salto";
    public static string PlayerTakeDamage;

    // Attacks

    public static string WarriorAttack;
    public static string WarriorAttackEnhanced;
    public static string MageAttack;
    public static string MageAttackEnhanced;
    public static string EnginAttack;
    public static string EnginAttackEnhanced;

    // Powers
    public static string MagePower = "VerdeMagic";
    public static string WarriorPower = "RojoMagic";
    public static string EngineerPower = "AmarilloMagic";

    // Items
    public static string GrabItem;
    public static string DropITem;
    public static string UseItemFail;
    public static string UseItemSuccessEffect;
    public static string UseItemSuccessCongrats;

    // XP
    public static string GrabExperience;

    // Music for DifferentScenes

    public static string Escena1 = "mEscena1";
    public static string Escena2 = "mEscena2";
    public static string Escena3 = "mEscena5";
    public static string Escena4 = "mEscena4";
    public static string Escena5 = "mEscena5";
    public static string Escena6 = "mEscena6";


    // Switches
    public static string SwitchOn;
    public static string SwitchOff;

    // Activables
    public static string GearActivate;
    public static string RunePlace;

    // NPC
    public static string NPCTalk;

    // Powerables
    public static string PowerableParticles;

    // Zones
    public static string ChatzoneParticles;

    // Portals
    public static string Portal;

    // Destroyables
    public static string DestroyBox;

    // Flames
    public static string FlamesJump;

    // EFfects
    public static string RockFall;
    public static string RockStomp;
    public static string TreeBreak;
    public static string Smoke;

    // Enemies
    public static string EnemyTakeDamage;
    public static string EnemyAttack;
    public static string EnemyDie;

    public static string SpiderTalk;
    public static string SpiderDown;
    public static string SpiderUp;
    public static string SpiderAttack;

    // Scene
    public static string SceneComplete;
    public static string ChangeScene;
    public static string GameOver;
    public static string Projectile;

    // Bubbles
    public static string Bubbles;

    // Diamonds
    public static string Diamonds;

}


public class SoundManager : MonoBehaviour
{

    public void PlaySound(GameObject gameObject, string soundName, bool loops)
    {
        AudioClip audioClip = Resources.Load("AudioClips/" + soundName) as AudioClip;

        if (audioClip == null)
        {
            Debug.LogError("Sound " + soundName + "does not exist");
            return;
        }

        AudioSource source = GetAudioSource(gameObject, audioClip, loops);

        source.Play();
    }

    public void PlaySound(GameObject gameObject, string soundName, bool loops, bool isBGMusic)
    {
        AudioClip audioClip = Resources.Load("AudioClips/" + soundName) as AudioClip;

        if (audioClip == null)
        {
            Debug.LogError("Sound " + soundName + "does not exist");
            return;
        }

        AudioSource source = GetAudioSource(gameObject, audioClip, loops);

        if (isBGMusic)
        {
            source.volume = .35f;
        }
        source.Play();
    }

    public void StopSound(GameObject gameObject, string soundName)
    {
        if (gameObject.GetComponent<AudioSource>())
        {
            AudioSource[] sSources = gameObject.GetComponents<AudioSource>();
            foreach (AudioSource aSource in sSources)
            {
                if (aSource.clip.name == soundName)
                {
                    if (aSource.isPlaying)
                    {
                        aSource.Stop();
                    }
                }
            }
        }
    }

    private AudioSource GetAudioSource(GameObject gameObject, AudioClip audioClip, bool loops)
    {
        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        AudioSource source = null;
        bool foundMySource = false;

        if (sources != null && sources.Length > 0)
        {
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i].clip.Equals(audioClip))
                {
                    source = sources[i];
                    foundMySource = true;
                    break;
                }
            }

            if (foundMySource == false)
            {
                source = gameObject.AddComponent<AudioSource>();
                source.clip = audioClip;
            }
        }
        else
        {
            source = gameObject.AddComponent<AudioSource>();
            source.clip = audioClip;
        }

        source.loop = loops;

        return source;
    }

}
