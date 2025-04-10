class GitHubSnippetDataService {
    constructor(repo = 'gauravt-cf/AutomateCore', folder = 'samples', filename = 'snippets.json') {
        this.url = `https://raw.githubusercontent.com/${repo}/main/${folder}/${filename}`;
        this.snippetsCache = null; // Will hold the data after first fetch
    }

    async getAllSnippets(forceRefresh = false) {
        if (this.snippetsCache && !forceRefresh) {
            return this.snippetsCache;
        }

        try {
            const response = await fetch(this.url);
            if (!response.ok) throw new Error(`Failed to fetch: ${response.status}`);
            const data = await response.json();
            this.snippetsCache = data;
            return data;
        } catch (error) {
            console.error('Error fetching all snippets:', error);
            return null;
        }
    }

    async getSnippetByKey(key, forceRefresh = false) {
        const data = await this.getAllSnippets(forceRefresh);
        if (data && data[key]) {
            return data[key];
        } else {
            console.warn(`Snippet key "${key}" not found.`);
            return null;
        }
    }
}
