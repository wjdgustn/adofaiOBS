const fs = require('fs')
const cp = require('child_process')
const yargs = require('yargs-parser')
const rimraf = require('rimraf')
const archiver = require('archiver')
const info = require('./Info.json')
const path = require('path')
// const { findSteamAppById } = require('find-steam-app')

setImmediate(async () => {
    const args = yargs(process.argv)

    rimraf.sync('Release')

    cp.execSync(`chcp 65001 && dotnet "C:\\Users\\Computer\\.dotnet\\sdk\\5.0.402\\MSBuild.dll" /p:Configuration=${args.release ? 'Release' : 'Debug'}`)

    fs.mkdirSync('Release')

    fs.copyFileSync(`./bin/${args.release ? 'Release' : 'Debug'}/${info.Id}.dll`, `./Release/${info.Id}.dll`)

    if (args.release) fs.copyFileSync('./Info.json', './Release/Info.json')
    else {
        const Info = JSON.parse(fs.readFileSync('./Info.json'))
        Info.Version += "_dev";

        fs.writeFileSync('./Release/Info.json', JSON.stringify(Info, null, 4))
    }
    
    fs.copyFileSync('../packages/Newtonsoft.Json.13.0.1/lib/net20/Newtonsoft.Json.dll', './Release/Newtonsoft.Json.dll')
    fs.copyFileSync('../packages/obs-websocket-dotnet.4.9.1/lib/netstandard2.0/obs-websocket-dotnet.dll', './Release/obs-websocket-dotnet.dll')
    fs.copyFileSync('../packages/WebSocketSharp-netstandard.1.0.1/lib/net45/websocket-sharp.dll', './Release/websocket-sharp.dll')
    fs.copyFileSync('../packages/netstandard.dll', './Release/netstandard.dll')

    if (args.release) {
        const zip = archiver('zip', {})
        const stream = fs.createWriteStream(path.join(__dirname, `${info.Id}-${info.Version}.zip`))
        zip.pipe(stream)
        zip.directory('Release/', info.Id)
        await zip.finalize()

        console.log('Successful.')
    } else {
        if(!args.norestart) await cp.exec('taskkill /f /im "a da"*')

        setTimeout(async () => {
            // const appPath = await findSteamAppById(977950)
            const appPath = `D:\\steam\\steamapps\\common\\A Dance of Fire and Ice`;
            const modPath = path.join(appPath, 'Mods', info.Id)
            if(!args.norestart) rimraf.sync(modPath)
            if(!args.norestart) fs.mkdirSync(modPath)
            fs.copyFileSync(`Release/${info.Id}.dll`, path.join(modPath, info.Id + '.dll'))
            fs.copyFileSync('Release/Info.json', path.join(modPath, 'Info.json'))

            if(!args.norestart) {
                fs.copyFileSync('../packages/Newtonsoft.Json.13.0.1/lib/net20/Newtonsoft.Json.dll', path.join(modPath, 'Newtonsoft.Json.dll'))
                fs.copyFileSync('../packages/obs-websocket-dotnet.4.9.1/lib/netstandard2.0/obs-websocket-dotnet.dll', path.join(modPath, 'obs-websocket-dotnet.dll'))
                fs.copyFileSync('../packages/WebSocketSharp-netstandard.1.0.1/lib/net45/websocket-sharp.dll', path.join(modPath, 'websocket-sharp.dll'))
                fs.copyFileSync('../packages/netstandard.dll', path.join(modPath, 'netstandard.dll'))
            }

            if(!args.norestart) try {
                await cp.exec('explorer steam://rungameid/977950')
            } catch (e) {
            }

            console.log('Successful.')
        }, 1200)
    }
})