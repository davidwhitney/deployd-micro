$install = Invoke-WebRequest -Uri https://raw.github.com/davidwhitney/deployd-micro/master/install.ps1
Invoke-Expression $install.Content
