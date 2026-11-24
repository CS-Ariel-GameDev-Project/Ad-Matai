
using System;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using Unity.RemoteConfig;

// Loads LiveOps settings from Unity Remote Config.
public class RangeRemoteConfig : MonoBehaviour
{
    public struct UserAttributes { }
    public struct AppAttributes { }

    public int MinWaveSize { get; private set; } = 3;
    public int MaxWaveSize { get; private set; } = 5;
    public float GreenTargetChance { get; private set; } = 0.33f;

    public bool IsReady { get; private set; }

    private bool fetchCompleted;

    // Log whether LiveOps settings came from the server or local data.
    private void OnFetchCompleted(ConfigResponse response)
    {
        fetchCompleted = true;

        Debug.Log(
            $"LIVEOPS SOURCE: {response.requestOrigin}"
        );

        switch (response.requestOrigin)
        {
            case ConfigOrigin.Remote:
                Debug.Log("LIVEOPS: Server responded successfully.");
                break;

            case ConfigOrigin.Cached:
                Debug.LogWarning(
                    "LIVEOPS: Using cached configuration."
                );
                break;

            case ConfigOrigin.Default:
                Debug.LogWarning(
                    "LIVEOPS: Using default configuration."
                );
                break;
        }
    }

    // Fetch LiveOps settings before the shooting range begins.
    private async void Start()
    {
        IsReady = false;
        fetchCompleted = false;

        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance
                    .SignInAnonymouslyAsync();
            }

            RemoteConfigService.Instance.FetchCompleted +=
                OnFetchCompleted;

            var config = await RemoteConfigService.Instance
                .FetchConfigsAsync(
                    new UserAttributes(),
                    new AppAttributes()
                );

            // Read the LiveOps parameters and validate their values.
            int min = config.GetInt("min_wave_size", 3);
            int max = config.GetInt("max_wave_size", 5);
            float green = config.GetFloat(
                "green_target_chance", 0.33f
            );

            Debug.Log(
                $"LIVEOPS FETCHED VALUES: " +
                $"Min={min}, Max={max}, Green={green}"
            );

            if (min >= 1 && max >= min &&
                green >= 0f && green <= 1f)
            {
                MinWaveSize = min;
                MaxWaveSize = max;
                GreenTargetChance = green;
            }
            else
            {
                Debug.LogWarning(
                    "LIVEOPS: Invalid parameters. Using defaults."
                );
            }

            Debug.Log(
                $"REMOTE CONFIG: Min={MinWaveSize}, " +
                $"Max={MaxWaveSize}, " +
                $"Green={GreenTargetChance}"
            );

            if (!fetchCompleted)
            {
                Debug.LogWarning(
                    "LIVEOPS: No fetch origin was reported."
                );
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                "LIVEOPS FETCH FAILED: " + e.Message
            );
        }
        finally
        {
            RemoteConfigService.Instance.FetchCompleted -=
                OnFetchCompleted;

            IsReady = true;
        }
    }
}
