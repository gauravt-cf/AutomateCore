window.addEventListener("scroll", () => {
    const btn = document.getElementById("scrollTopBtn");
    if (btn) {
        btn.classList.toggle("hidden", window.scrollY < 200);
    }
});


function getPageUrl(pageUrl) {
    const basePath = window.location.pathname.split("/")[1];
    const pathPrefix = basePath && basePath !== "" ? `/${basePath}` : "";
    return `${pathPrefix}/${pageUrl}`.replace(/\/{2,}/g, "/");
}
$(function () {
    let currentYear = new Date().getFullYear();
    var AutomateCoreUI = new AutomateCoreUIComponents();
    $('top-header-bar').html(AutomateCoreUI.getTopHeader());
    $('button-scroll').html(AutomateCoreUI.getScrollButton());
    $('automate-core-footer').html(AutomateCoreUI.getMainFooter());
    $('current-year').html(currentYear);
    $('a[data-page-url]').each(function () {
        const page = $(this).data('page-url');
        $(this).attr('href', getPageUrl(page));
    });
});
function goToHomePage() {
    const pathParts = window.location.pathname.split("/");
    const basePath = pathParts[1]; // For /my-site/...
    window.location.href = `/${basePath}/index.html`;
}

class AutomateCoreUIComponents {
    constructor() {
        this.downloadPageUrl = () => {
            const basePath = window.location.pathname.split("/")[1]; // repo-name
            return `/${basePath}/downloads`;
        };
        this.topHeader = {
            "html": `<header class="bg-gradient-to-r from-gray-900 via-blue-900 to-blue-300 text-white py-4 shadow-xl border-b border-cyan-500/20 relative z-50">
  <div class="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center px-6 gap-4">

    <!-- Left: Logo + Title -->
    <div class="flex items-center gap-4 relative z-10">
      <!-- Animated Logo with Glow Effect -->
      <div class="relative group" onclick="goToHomePage()" style="cursor: pointer;">
        <div class="absolute inset-0 bg-cyan-500 rounded-full blur-md group-hover:blur-lg transition-all duration-500 opacity-70 group-hover:opacity-100"></div>
        <div class="relative backdrop-blur-sm bg-white/5 p-3 rounded-full border border-cyan-400/30 shadow-lg hover:shadow-cyan-400/40 transition-all duration-300">
          <svg class="w-8 h-8 text-cyan-300 group-hover:text-white group-hover:scale-110 transition-transform" fill="none" stroke="currentColor" stroke-width="1.5"
            viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round"
              d="M13 10V3L4 14h7v7l9-11h-7z"></path>
          </svg>
        </div>
      </div>

      <!-- Title & Subtitle -->
      <div class="relative">
       <h1 class="text-2xl md:text-3xl font-bold tracking-tight bg-clip-text bg-gradient-to-r from-cyan-400 to-blue-500 drop-shadow-lg">
  AutomateCore
</h1>

        <p class="text-xs text-cyan-200/90 tracking-wider  uppercase">Config-Driven Task Scheduler</p>
        <div class="absolute -bottom-1 left-0 h-0.5 bg-gradient-to-r from-cyan-400 to-blue-500 w-full"></div>
      </div>
    </div>

    <!-- Right: Buttons + Navigation -->
    <div class="flex flex-col md:flex-row items-center gap-4">

      <!-- GitHub Stats: Stars, Forks, License -->
      <div class="hidden md:flex items-center gap-2 ml-4  text-xs text-gray-300">
        <div class="flex items-center gap-1 bg-gray-800/80 px-2 py-1 rounded-md border border-gray-700" id="github-stars">
          ⭐ <span>--</span>
        </div>
        <div class="flex items-center gap-1 bg-gray-800/80 px-2 py-1 rounded-md border border-gray-700" id="github-forks">
          🍴 <span>--</span>
        </div>
        <div class="flex items-center gap-1 bg-gray-800/80 px-2 py-1 rounded-md border border-gray-700" id="github-license">
          📜 <span>--</span>
        </div>
      </div>

      <!-- GitHub CTA Button -->
      <a href="https://github.com/gauravt-cf/AutomateCore" target="_blank"
        class="relative overflow-hidden group bg-gradient-to-r from-cyan-500/90 to-blue-600/90 text-white font-medium px-5 py-2 rounded-lg shadow-lg hover:shadow-cyan-400/30 transition-all duration-300 flex items-center gap-2 border border-cyan-400/30 hover:border-cyan-400/50">
        <svg class="w-5 h-5 group-hover:animate-pulse" fill="currentColor" viewBox="0 0 24 24">
          <path d="M12 0C5.37 0 0 5.373 0 12c0 5.304 3.438 9.8 8.207 11.387.6.113.793-.26.793-.577..."></path>
        </svg>
        <span class="text-sm">View on GitHub</span>
        <span class="absolute inset-0 bg-gradient-to-r from-cyan-400/20 to-blue-500/20 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></span>
      </a>

      <!-- Navigation Menu -->
      <nav class="flex gap-4 bg-gray-900/80 backdrop-blur-sm px-3 py-1.5 rounded-full border border-gray-700/50">
        <a href="${this.downloadPageUrl()}" class="text-xs  text-cyan-300 hover:text-white px-3 py-1 rounded-full hover:bg-cyan-900/30 transition-all duration-200 flex items-center gap-1">
          ⬇️ Downloads
        </a>
        <a href="${getPageUrl("tools")}" class="text-xs  text-cyan-300 hover:text-white px-3 py-1 rounded-full hover:bg-cyan-900/30 transition-all duration-200 flex items-center gap-1">
          🛠 Tools
        </a>
        <a href="${getPageUrl("docs")}" class="text-xs text-cyan-300 hover:text-white px-3 py-1 rounded-full hover:bg-cyan-900/30 transition-all duration-200 flex items-center gap-1">
          📘 Docs
        </a>
      </nav>
    </div>
  </div>
</header>

<!-- GitHub Stats Script -->
<script>
  fetch('https://api.github.com/repos/gauravt-cf/AutomateCore')
    .then(res => res.json())
    .then(data => {
      document.querySelector('#github-stars span').textContent = data.stargazers_count;
      document.querySelector('#github-forks span').textContent = data.forks_count;
      document.querySelector('#github-license span').textContent = data.license?.spdx_id || 'N/A';
    })
    .catch(console.error);
</script>
`
        },
            this.buttonScroll = {
                "html": `<button onclick="window.scrollTo({top: 0, behavior: 'smooth'})"
                        class="fixed bottom-6 right-6 bg-gradient-to-tr from-blue-500 to-cyan-500 text-white p-4 rounded-full shadow-xl hover:shadow-2xl hover:scale-110 transition-transform duration-300 z-50 hidden group"
                        id="scrollTopBtn" aria-label="Scroll to top">
                        <svg xmlns="http://www.w3.org/2000/svg"
                            class="w-5 h-5 transition-transform duration-300 group-hover:-translate-y-1" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor" stroke-width="2">
                            <path stroke-linecap="round" stroke-linejoin="round" d="M5 15l7-7 7 7" />
                        </svg>
                    </button>`
            },
            this.footer = {
                Commonhtml: `<!-- Add this right before your closing </body> tag -->
<footer class="bg-gray-900 text-white py-12 px-6 relative z-10">
  <div class="max-w-7xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-8">
    <div>
      <h3 class="text-xl font-bold mb-4 gradient-text">AutomateCore</h3>
      <p class="text-gray-400">Lightweight, config-driven task scheduler for .NET applications.</p>
    </div>
    <div>
      <h4 class="font-semibold mb-4">Resources</h4>
      <ul class="space-y-2">
        <li><a href="#" class="text-gray-400 hover:text-white transition">Documentation</a></li>
        <li><a href="#" class="text-gray-400 hover:text-white transition">GitHub</a></li>
        <li><a href="#" class="text-gray-400 hover:text-white transition">Examples</a></li>
      </ul>
    </div>
    <div>
      <h4 class="font-semibold mb-4">Community</h4>
      <ul class="space-y-2">
        <li><a href="#" class="text-gray-400 hover:text-white transition">Discussions</a></li>
        <li><a href="#" class="text-gray-400 hover:text-white transition">Issues</a></li>
        <li><a href="#" class="text-gray-400 hover:text-white transition">Contributing</a></li>
      </ul>
    </div>
    <div>
      <h4 class="font-semibold mb-4">Connect</h4>
      <div class="flex space-x-4">
        <a href="#" class="text-gray-400 hover:text-white transition"><i class="fab fa-github fa-lg"></i></a>
        <a href="#" class="text-gray-400 hover:text-white transition"><i class="fab fa-twitter fa-lg"></i></a>
        <a href="#" class="text-gray-400 hover:text-white transition"><i class="fab fa-discord fa-lg"></i></a>
      </div>
    </div>
  </div>
  <div class="max-w-7xl mx-auto mt-12 pt-6 border-t border-gray-800 text-center text-gray-500 text-sm">
    <p>© 2023 AutomateCore. All rights reserved.</p>
  </div>
</footer>`,

                404: `<footer class="absolute bottom-4 text-sm text-gray-400">
                            &copy; <current-year></current-year> AutomateCore Scheduler
                        </footer>`
            };
    }
    getTopHeader() {
        return this.topHeader.html;
    }
    getScrollButton() {
        return this.buttonScroll.html;
    }
    getMainFooter() {
        return this.footer.Commonhtml;
    }
    get404Footer() {
        return this.footer[404];
    }
}
function showErrorBanner(message = 'Something went wrong while loading content.', duration = 5000) {
    const banner = document.getElementById('errorBanner');
    if (banner) {
        banner.textContent = `🚨 ${message}`;
        banner.classList.remove('hidden');

        // Auto-hide after duration
        setTimeout(() => {
            banner.classList.add('hidden');
        }, duration);
    }
}
function hideErrorBanner() {
    const banner = document.getElementById('errorBanner');
    if (banner) {
        banner.classList.add('hidden');
    }
}
