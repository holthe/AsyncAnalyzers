require 'fileutils'
require 'albacore'
require 'albacore/task_types/test_runner'
require 'semver'

# Utility functions
def set(key, value)
    if value == false
        value = 'FALSE'
    elsif value == true
        value = 'TRUE'
    end
    ENV[key.to_s.upcase] = value
end

def fetch(key)
    val = ENV[key.to_s.upcase]
    if 'FALSE' == val
        val = false
    elsif 'TRUE' == val
        val = true
    end
    val
end

if File.exists? ('rake_env')
    load 'rake_env'
else
    puts "Using environment variables."
end

@current_dir = File.dirname(__FILE__)

@solution = 'AsyncAnalyzers.sln'
@build_configuration = 'Release'

@xunit_console = './packages/xunit.runner.console.2.2.0/tools/xunit.console.exe'
@xunit_test_assemblies = "**/*AsyncAnalyzers.Test/bin/#{@build_configuration}/*AsyncAnalyzers.Test.dll"

# These values will come from either the file rake_env or environment variables
@nuget_api_key = fetch(:NuGetApiKey)
@nuget_source = fetch(:NuGetSource)

@nuspec_file = './AsyncAnalyzers/AsyncAnalyzers.nuspec'
@nuspec_version = ''

task :default => [:restore, :build_solution, :xunit_tests]

desc 'restore all nugets as per the packages.config files'
nugets_restore :restore do |p|
  p.out = 'packages'
  p.exe = 'nuget.exe'
end

desc 'Remove the bin and obj directories.'
task :clean do
    rm_rf 'AsyncAnalyzers/bin'
    rm_rf 'AsyncAnalyzers/obj'
    rm_rf 'AsyncAnalyzers.Test/bin'
    rm_rf 'AsyncAnalyzers.Test/obj'
end

desc 'Clean and rebuild the solution for @build_configuration configuration.'
build :build_solution => [:clean] do |b|
    b.sln = @solution               # the solution to build
    b.target = ['Clean', 'Rebuild'] # call with an array of targets or just a single target
    b.prop 'Configuration', "#{@build_configuration}" # call with 'key, value', to specify an MsBuild property
    b.clp 'ShowEventId'             # parameters for the console logger of MsBuild
    b.logging = 'detailed'          # available verbosity levels are: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]. Opposite: b.be_quiet
    b.nologo                        # no Microsoft/XBuild header output
end

desc 'Run xUnit tests'
test_runner :xunit_tests do |tests|
    tests.exe = @xunit_console
    tests.files = FileList[@xunit_test_assemblies]
end

task :update_version do
    text = File.read(@nuspec_file)
    
    ver = SemVer.find
    @nuspec_version = "#{SemVer.new(ver.major, ver.minor, ver.patch).format "%M.%m.%p"}.0"
    new_contents = text.gsub(/(?<=\<version\>).+(?=\<\/version\>)/, @nuspec_version)
    
    File.open(@nuspec_file, "w") {|file| file.puts new_contents }
end

desc 'Pack and push NuGet package'
task :nuget_pack_and_push => [:update_version] do
    pack_command = "nuget pack #{@nuspec_file} -Verbosity detailed"
    sh "#{pack_command}", verbose: false do |ok, status|
        unless ok
            raise Exception.new("[!] Failed to pack NuGet package with status #{status.exitstatus}")
        end
    end
    
    push_command = "nuget push ./AsyncAnalyzers.#{@nuspec_version}.nupkg -Verbosity detailed -ApiKey #{@nuget_api_key} -Source #{@nuget_source}"
    sh "#{push_command}", verbose: false do |ok, status|
        unless ok
            raise Exception.new("[!] Failed to push NuGet package with status #{status.exitstatus}")
        end
    end
end