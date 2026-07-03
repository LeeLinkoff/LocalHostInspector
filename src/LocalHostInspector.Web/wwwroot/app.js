async function loadDiagnostics()
{
    try
    {
        const response = await fetch("/api/info");

        if (!response.ok) {
            const text = await response.text();
            throw new Error(text);
        }

        const result = await response.json();

        //
        // Request
        //
        document.getElementById("url").textContent = result.url ?? "";
        document.getElementById("host").textContent = result.host ?? "";
        document.getElementById("conclusion").textContent = result.conclusion ?? ""; 

        //
        // Evidence
        //
        const evidence = document.getElementById("evidence");
        evidence.innerHTML = "";

        if (result.evidence?.length)
        {
            for (const item of result.evidence)
            {
                const li = document.createElement("li");
                li.textContent = item;
                evidence.appendChild(li);
            }
        }
        else
        {
            const li = document.createElement("li");
            li.textContent = "No supporting evidence available.";
            evidence.appendChild(li);
        }

        //
        // Hosts File
        //
        if (result.hostsFile?.matches?.length)
        {

            let output = "";
            output += `File       : ${result.hostsFile.filePath}\n`;
            if (result.hostsFile.lineNumber != null)
            {
                output += `Line       : ${result.hostsFile.lineNumber}\n\n`;
            }
            if (result.hostsFile.matches?.length)
            {
                output += result.hostsFile.matches.join("\n");
            }
            else
            {
                output += "\nNo matching hosts file entries found.";
            }
            document.getElementById("hostsFile").textContent = output;
        }
        else
        {

            document.getElementById("hostsFile").textContent = "No matching hosts file entries found.";
        }

        //
        // Web Server
        //
        if (result.webServer)
        {
            let output = "";

            output += `Server Type : ${result.webServer.serverType ?? ""}\n`;
            output += `Site Name   : ${result.webServer.siteName ?? ""}\n`;
            output += `Binding     : ${result.webServer.binding ?? ""}\n`;
            output += `Matched     : ${result.webServer.isMatch}`;

            if (result.webServer.evidence?.length)
            {

                output += "\n\nEvidence\n";
                output += "--------\n";

                for (const item of result.webServer.evidence) {
                    output += `${item}\n`;
                }
            }

            document.getElementById("webServer").textContent = output;
        }

        //
        // DNS
        //
        document.getElementById("systemDns").textContent = result.dns?.systemLookup ?? "";
        document.getElementById("googleDns").textContent = result.dns?.googleLookup ?? "";
        document.getElementById("cloudflareDns").textContent = result.dns?.cloudflareLookup ?? "";

        //
        // Authoritative DNS Provider
        //
        document.getElementById("authoritativeProvider").textContent = result.dns?.authoritativeProvider?.organization ?? "Unknown";
        if (result.dns?.authoritativeProvider) {
            const p = result.dns.authoritativeProvider;
            let output = "";
            output += `Organization : ${p.organization ?? ""}\n`;
            output += `Network      : ${p.network ?? ""}\n`;
            output += `IP Address   : ${p.address ?? ""}\n`;
            if (p.email)
                output += `Email        : ${p.email}\n`;
            if (p.phone)
                output += `Phone        : ${p.phone}\n`;
            if (p.abuseEmail)
                output += `Abuse Email  : ${p.abuseEmail}\n`;
            if (p.abusePhone)
                output += `Abuse Phone  : ${p.abusePhone}\n`;
            document.getElementById("authoritativeProvider").textContent = output;
        }
        else {
            document.getElementById("authoritativeProvider").textContent = "No provider information available.";
        }

        //
        // Authoritative Name Servers
        //
        if (result.dns?.authoritativeNameServers?.length)
        {
            document.getElementById("authoritativeNameServers").textContent = result.dns.authoritativeNameServers.join("\n");
        }
        else {
            document.getElementById("authoritativeNameServers").textContent = "No authoritative name servers found.";
        }

        //
        // Raw Authoritative DNS
        //
        document.getElementById("authoritativeDns").textContent = result.dns?.authoritativeLookup ?? "";
    }
    catch (error)
    {
        alert(error.stack);

        document.getElementById("conclusion").textContent = "Unable to retrieve diagnostic information.";

        document.getElementById("evidence").innerHTML = `<li>${error}</li>`;
    }
}

document.addEventListener("DOMContentLoaded", loadDiagnostics);