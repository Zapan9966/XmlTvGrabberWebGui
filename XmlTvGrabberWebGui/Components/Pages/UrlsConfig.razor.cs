using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XmlTvGrabberWebGui.Models;

namespace XmlTvGrabberWebGui.Components.Pages
{
    public partial class UrlsConfig
    {
        private XmlUrl NewUrl { get; set; } 

        private string StatusMessage { get; set; }

        protected override Task OnInitializedAsync()
        {
            NewUrl = new XmlUrl { Index = context.XmlUrls.Max(x => x.Index) + 1 };

            return base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                jsRuntime.InvokeVoidAsync("onBlazorReady");

            return base.OnAfterRenderAsync(firstRender);
        }

        private async Task Save(EditContext editContext)
        {
            var url = (XmlUrl)editContext.Model;
            if (url != null)
            {
                context.Update(url);
                await context.SaveChangesAsync();

                StatusMessage = url.XmlUrlId > 0 ? $"URL modifiée avec succés !" : null;
                NewUrl = new XmlUrl { Index = context.XmlUrls.Max(x => x.Index) + 1 };
            }
        }

        private async Task Up(int id)
        {
            // Récupération de l'URL à modifier
            var url = await context.XmlUrls.FindAsync(id);
            if (url != null)
            {
                if (url.Index > 1)
                {
                    // Récupération de l'URL précédente
                    var prevUrl = context.XmlUrls.FirstOrDefault(x => x.Index == url.Index - 1);
                    if (prevUrl != null)
                    {
                        try
                        {
                            prevUrl.Index++;
                            url.Index--;
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            StatusMessage = $"Erreur: {ex.Message}";
                        }
                    }
                    else
                        StatusMessage = $"Erreur: URL index '{url.Index - 1}' non trouvé";
                }
            }
            else
                StatusMessage = $"Erreur: URL id '{id}' non trouvée";
        }

        private async Task Down(int id)
        {
            // Récupération de l'URL à modifier
            var url = await context.XmlUrls.FindAsync(id);
            if (url != null)
            {
                if (url.Index < context.XmlUrls.Max(x => x.Index))
                {
                    // Récupération de l'URL précédente
                    var nextUrl = context.XmlUrls.FirstOrDefault(x => x.Index == url.Index + 1);
                    if (nextUrl != null)
                    {
                        try
                        {
                            nextUrl.Index--;
                            url.Index++;
                            await context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            StatusMessage = $"Erreur: {ex.Message}";
                        }
                    }
                    else
                        StatusMessage = $"Erreur: URL index '{url.Index + 1}' non trouvé";
                }
            }
            else
                StatusMessage = $"Erreur: URL id '{id}' non trouvée";
        }

        private async Task Delete(int id)
        {
            // Récupération de l'URL à supprimer
            var url = await context.XmlUrls.FindAsync(id);
            if (url != null)
            {
                try
                {
                    context.XmlUrls.Remove(url);
                    await context.SaveChangesAsync();
                    StatusMessage = $"URL '{url.Url}' supprimée !";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Erreur: {ex.Message}";
                }
            }
            else
                StatusMessage = $"Erreur: URL id '{id}' non trouvée";
        }


    }
}
