
window.addEventListener("scroll", () => {
    document.getElementById("scrollTopBtn").classList.toggle("hidden", window.scrollY < 200);
});
$(function () {
    let currentYear = new Date().getFullYear();
    var AutomateCoreUI = new AutomateCoreUIComponents();
    $('top-header-bar').html(AutomateCoreUI.getTopHeader());
    $('button-scroll').html(AutomateCoreUI.getScrollButton());
    $('automate-core-footer').html(AutomateCoreUI.getMainFooter());
    $('current-year').html(currentYear);
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
            "html": `<header class="bg-gradient-to-r from-blue-800 to-cyan-600 text-white py-6 shadow-lg">
                        <div class="max-w-7xl mx-auto flex flex-col md:flex-row justify-between items-center px-6 gap-4">
                            
                            <!-- Left: Logo + Title -->
                            <div class="flex items-center gap-4">
                            
                            <!-- Logo with Glassmorphism -->
                            <div class="backdrop-blur-sm bg-white/10 p-2 rounded-full shadow-md hover:shadow-white/30 transition duration-300" onclick="goToHomePage()" style="cursor: pointer;">
                                <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" stroke-width="2"
                                viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M13 10V3L4 14h7v7l9-11h-7z"></path>
                                </svg>
                            </div>

                            <!-- Title & Subtitle -->
                            <div>
                                <h1 class="text-3xl md:text-4xl font-extrabold tracking-wide text-white drop-shadow">
                                AutomateCore Scheduler
                                </h1>
                                <p class="text-sm text-white text-opacity-80">Config-driven .NET Task Automation</p>
                            </div>

                            <!-- GitHub Star Badge -->
                            <div class="flex flex-wrap items-center gap-3 ml-2 transition-transform duration-300 hover:scale-105">
                                <a href="https://github.com/gauravt-cf/AutomateCore" target="_blank">
                                    <img src="https://img.shields.io/github/stars/gauravt-cf/AutomateCore?style=social" alt="GitHub stars" />
                                </a>
                                <a href="https://github.com/gauravt-cf/AutomateCore/network/members" target="_blank">
                                    <img src="https://img.shields.io/github/forks/gauravt-cf/AutomateCore?style=social" alt="GitHub forks" />
                                </a>
                                <a href="https://github.com/gauravt-cf/AutomateCore/blob/main/LICENSE" target="_blank">
                                    <img src="https://img.shields.io/github/license/gauravt-cf/AutomateCore" alt="License" />
                                </a>
                            <a href="https://github.com/gauravt-cf/AutomateCore/issues" target="_blank">
                                    <img src="https://img.shields.io/github/issues/gauravt-cf/AutomateCore" alt="Open Issues" />
                                </a>
                            </div>
                            </div>

                           <!-- Right: GitHub CTA Button + Menu -->
                            <div class="flex flex-col md:flex-row items-center gap-4">

                                <!-- GitHub CTA Button -->
                                <a href="https://github.com/gauravt-cf/AutomateCore" target="_blank"
                                    class="bg-gradient-to-r from-cyan-400 to-blue-500 text-white font-semibold px-6 py-2 rounded-full shadow-lg hover:scale-105 hover:shadow-blue-400/40 transition duration-300 flex items-center gap-2">
                                    <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                                        <path
                                            d="M12 0C5.37 0 0 5.373 0 12c0 5.304 3.438 9.8 8.207 11.387.6.113.793-.26.793-.577 0-.285-.012-1.233-.018-2.236-3.338.727-4.042-1.612-4.042-1.612-.546-1.385-1.333-1.754-1.333-1.754-1.09-.745.083-.729.083-.729 1.204.085 1.837 1.237 1.837 1.237 1.07 1.832 2.809 1.303 3.495.996.108-.775.42-1.303.763-1.602-2.665-.303-5.467-1.332-5.467-5.932 0-1.31.467-2.381 1.235-3.221-.124-.303-.536-1.524.117-3.176 0 0 1.008-.322 3.3 1.23a11.51 11.51 0 013.003-.404c1.02.004 2.047.138 3.003.404 2.289-1.552 3.294-1.23 3.294-1.23.655 1.653.243 2.874.12 3.176.77.84 1.233 1.911 1.233 3.221 0 4.61-2.807 5.625-5.48 5.922.43.372.823 1.102.823 2.222 0 1.604-.015 2.896-.015 3.289 0 .32.19.694.8.576C20.565 21.796 24 17.302 24 12c0-6.627-5.373-12-12-12z">
                                        </path>
                                    </svg>
                                    View on GitHub
                                </a>

                                <!-- Navigation Menu -->
                                <nav class="flex gap-4">
                                    <a href="${this.downloadPageUrl()}" class="text-black text-sm font-medium hover:underline hover:text-cyan-200 transition">
                                        Downloads
                                    </a>
                                    <!-- Add more links here if needed -->
                                    <!-- <a href="/docs" class="text-white text-sm font-medium hover:underline hover:text-cyan-200 transition">
                                        Docs
                                    </a> -->
                                </nav>

                            </div>

                        </div>
                    </header>`
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
                Commonhtml: `<footer class="bg-gradient-to-r from-gray-900 to-gray-800 text-white py-8 text-center">
                                <p class="text-sm">&copy; <current-year></current-year> AutomateCore Scheduler — Built with ❤️ for Windows services</p>
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
    get404Footer(){
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
