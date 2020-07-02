using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace OutilsBlazor
{
    public abstract class Page<T> : Base where T : class
    {
        [Parameter]
        public string SetNum { get; set; }

        public T Item { get; set; }

        protected override Task OnInitializedAsync()
        {
            Item = GetItem();
            return Task.CompletedTask;
        }

        public abstract T GetItem();
    }
}