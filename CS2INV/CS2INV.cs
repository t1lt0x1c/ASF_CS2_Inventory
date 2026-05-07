using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using JetBrains.Annotations;

namespace CS2INV;

#pragma warning disable CA1812 // ASF uses this class during runtime
[UsedImplicitly]
internal sealed class CS2INV :
	IGitHubPluginUpdates,
	IBotCommand2            // Обработка пользовательских команд
	{
	public string Name => nameof(CS2INV);
	public string RepositoryName => "t1lt0x1c/ASF_CS2_Inventory";
	public Version Version => typeof(CS2INV).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

	public Task OnLoaded() {
		ASF.ArchiLogger.LogGenericInfo($"Hello {Name}!");

		return Task.CompletedTask;
	}

	public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0) {

		if (args.Length == 0) {
			return null;
		}

		switch (args[0].ToUpperInvariant()) {
			case "INV" when access >= EAccess.FamilySharing:
				_ = GetInventory(bot);
				return null;
			default:
				return null;
		}
	}

	public async Task<List<Asset>> GetInventory(Bot bot) {
		List<Asset> result = [];

		ASF.ArchiLogger.LogGenericInfo("Инвертарь бота " + bot.BotName);
		ASF.ArchiLogger.LogGenericInfo("Оружие:");

		Dictionary<string, int> caseCounts = [];

		await foreach (Asset item in bot.ArchiHandler.GetMyInventoryAsync(730, 2).ConfigureAwait(true)) {
			if (item.Description != null) {
				string name = item.Description.MarketHashName;
				if (item.Description.Tags.Any(tag => tag.Identifier == "Type" && tag.Value == "CSGO_Type_WeaponCase")) {
					caseCounts[name] = caseCounts.TryGetValue(name, out int value) ? ++value : 1;
				} else {
					ASF.ArchiLogger.LogGenericInfo(name);
				}
			}
		}
		ASF.ArchiLogger.LogGenericInfo("Кейсы:");
		foreach (KeyValuePair<string, int> case_ in caseCounts) {
			ASF.ArchiLogger.LogGenericInfo(case_.Key + " x" + case_.Value);
		}

		ASF.ArchiLogger.LogGenericInfo($"Всего предметов: {result.Count}, кейсов: {caseCounts.Values.Sum()}");
		return result;
	}
}
#pragma warning restore CA1812 // ASF uses this class during runtime
